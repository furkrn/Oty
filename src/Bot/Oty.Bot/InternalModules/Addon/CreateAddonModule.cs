namespace Oty.Bot.InternalModules;

public partial class AddonManager
{
    public sealed class CreateAddonModule : BaseVerifiedCommandModule<SlashInteractionContext>
    {
        private readonly OtyInteractivity _interactivity;

        public CreateAddonModule(SlashInteractionContext context) : base(context)
        {
            this._interactivity = context.Client.GetInteractivityExtension();
        }

        public const string MESSAGE_CT_BUTTONID = "messagect_button";

        public const string SLASH_BUTTONID = "slash_button";

        public const string USERCT_BUTTONID = "userct_button";

        public const string CONTINUE_BUTTONID = "continue_button";

        public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
        {
            var (componentInteraction, success) = await this.HandleCommandMetadataDialogAsync();

            if (!success)
            {
                return;
            }
        }

        private async Task<Result<ComponentInteractionCreateEventArgs>> HandleCommandMetadataDialogAsync()
        {
            DiscordEmbed commandEmbed = new DiscordEmbedBuilder()
                .WithDescription("I know coding a module is kinda hard move to do but what about creating your 'special requirement' module by clicking buttons.");

            var messageCommandButton = new DiscordButtonComponent(ButtonStyle.Primary, MESSAGE_CT_BUTTONID, "Message Context", false);
            var slashCommandButton = new DiscordButtonComponent(ButtonStyle.Primary, SLASH_BUTTONID, "Slash", true);
            var userCommandButton = new DiscordButtonComponent(ButtonStyle.Primary, USERCT_BUTTONID, "User Context", false);

            var editButton = new DiscordButtonComponent(ButtonStyle.Success, "edit_button", "Edit Names", false);

            var discardButton = new DiscordButtonComponent(ButtonStyle.Danger, "discard_button", "Discard", false);
            var continueButton = new DiscordButtonComponent(ButtonStyle.Success, CONTINUE_BUTTONID, "Continue", true);

            var buttonCollections = new[]
            {
                new[] { messageCommandButton, slashCommandButton, userCommandButton },
                new[] { editButton },
                new[] { discardButton, continueButton },
            };

            var baseInteractionResponseBuilder = new DiscordInteractionResponseBuilder()
                .AddEmbed(commandEmbed);

            var interactionResponseBuilder = new DiscordInteractionResponseBuilder(baseInteractionResponseBuilder)
                .AddComponents(messageCommandButton, slashCommandButton, userCommandButton)
                .AddComponents(editButton)
                .AddComponents(discardButton, continueButton);

            await this.Context.CreateResponseAsync(interactionResponseBuilder);
            var message = await this.Context.GetOriginalResponseAsync();

            using var componentCancellationToken = new CancellationTokenSource();
            componentCancellationToken.CancelAfter(TimeSpan.FromHours(3));

            var componentInteractivity = new CommandMetadataDialogButtonComponentHandler(componentCancellationToken, baseInteractionResponseBuilder,
                buttonCollections, this.Context);

            var interactivityRequest = new ComponentInteractivityBuilder()
                .AsRepeative()
                .WithTargetMessage(message)
                .WithTargetUser(this.Context.User)
                .AddTargetComponent(messageCommandButton, componentInteractivity.SelectCommandTypeAsync)
                .AddTargetComponent(slashCommandButton, componentInteractivity.SelectCommandTypeAsync)
                .AddTargetComponent(userCommandButton, componentInteractivity.SelectCommandTypeAsync)
                .AddTargetComponent(editButton, componentInteractivity.EditNamesAsync)
                .AddTargetComponent(discardButton, componentInteractivity.CancelAsync)
                .AddTargetComponent(continueButton, componentInteractivity.ContinueAsync);

            var result = await this._interactivity.HandleComponentInteraction(interactivityRequest, componentCancellationToken.Token);

            bool isCompleted = componentInteractivity.Completed;

            if (!isCompleted)
            {
                await this.Context.DeleteOriginalResponseAsync();
            }

            return new(isCompleted, result.EventResult);
        }

        private sealed class CommandMetadataDialogButtonComponentHandler
        {
            private static readonly IReadOnlyDictionary<string, ApplicationCommandType> s_componentTypes = new Dictionary<string, ApplicationCommandType>
            {
                {MESSAGE_CT_BUTTONID, ApplicationCommandType.MessageContextMenu},
                {SLASH_BUTTONID, ApplicationCommandType.SlashCommand},
                {USERCT_BUTTONID, ApplicationCommandType.UserContextMenu}
            };

            private readonly CancellationTokenSource _cancellationTokenSource;

            private readonly DiscordInteractionResponseBuilder _interactionResponseBuilder;

            private readonly DiscordButtonComponent[][] _buttonsCollection;

            private readonly OtyInteractivity _interactivity;

            private readonly SlashInteractionContext _context;

            public CommandMetadataDialogButtonComponentHandler(CancellationTokenSource cancellationTokenSource, DiscordInteractionResponseBuilder interactionResponseBuilder,
                DiscordButtonComponent[][] buttons, SlashInteractionContext context)
            {
                this._cancellationTokenSource = cancellationTokenSource;
                this._interactionResponseBuilder = interactionResponseBuilder;
                this._buttonsCollection = buttons;
                this._interactivity = context.Client.GetInteractivityExtension();
                this._context = context;
            }

            public bool Completed { get; private set; }

            public ApplicationCommandType CommandType { get; private set; }

            public string? CommandName { get; private set; }

            public string? CommandDescription { get; private set; }

            public Task CancelAsync(ComponentInteractionCreateEventArgs eventArgs)
            {
                this._cancellationTokenSource.Cancel();

                return eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
            }

            public Task ContinueAsync(ComponentInteractionCreateEventArgs eventArgs)
            {
                this.Completed = true;

                return this.CancelAsync(eventArgs);
            }

            public async Task EditNamesAsync(ComponentInteractionCreateEventArgs eventArgs)
            {
                string interactionId = $"mcdg_{this._context.User.Id}";
                string nameTextBox = "c_name";
                string descriptionTextBox = "c_desc";

                var modalRequestInteractionBuilder = new DiscordInteractionResponseBuilder()
                    .WithTitle("The modal request.")
                    .WithCustomId(interactionId)
                    .AddComponents(new TextInputComponent("d", nameTextBox, "gg", null, true, TextInputStyle.Short, 1, 25));

                if (this.CommandType is ApplicationCommandType.SlashCommand)
                {
                    modalRequestInteractionBuilder.AddComponents(new TextInputComponent("g", descriptionTextBox, "dd", null, true, TextInputStyle.Paragraph));
                }

                await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalRequestInteractionBuilder);

                var modalRequest = new ModalRequest(this._context.User, interactionId);
                var modalResult = await this._interactivity.HandleModalRequestAsync(modalRequest, TimeSpan.FromHours(1.5));

                if (modalResult.IsTimedOut)
                {
                    return;
                }

                this.CommandName = modalResult.Entries[nameTextBox];
                
                if (modalResult.Entries.TryGetValue(descriptionTextBox, out var description))
                {
                    this.CommandDescription = description;
                }

                this._buttonsCollection[2][1].Enable();

                await this.UpdateMessageAsync(modalResult.EventResult);
            }

            public Task SelectCommandTypeAsync(ComponentInteractionCreateEventArgs eventArgs)
            {
                foreach (var button in this._buttonsCollection[0])
                {
                    if (button.CustomId == eventArgs.Id)
                    {
                        button.Disable();
                    }
                    else
                    {
                        button.Enable();
                    }
                }

                this.CommandType = s_componentTypes[eventArgs.Id];

                return this.UpdateMessageAsync(eventArgs);
            }

            private Task UpdateMessageAsync(InteractionCreateEventArgs eventArgs)
            {
                var interactionResponseBuilder = new DiscordInteractionResponseBuilder(this._interactionResponseBuilder);

                foreach (var buttons in this._buttonsCollection)
                {
                    interactionResponseBuilder.AddComponents(buttons);
                }

                return eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, interactionResponseBuilder);
            }
        }
    }
}