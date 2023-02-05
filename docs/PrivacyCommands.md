## Privacy commands

**This documentation may change on future because bot is not even on its v0.5 release**

### Command structure

```
privacy
|--- mydata *
|--- guilddata *
|--- delete
    |--- user *
    |--- guild *
```

### Executable Commands

#### `/privacy mydata`
- Shows what data we collected from you.

##### Cooldowns & Limitations
- 5 minute user cooldown right after executing the command once.

#### `/privacy guilddata`
Shows what data we collected from the specified guild

##### Cooldowns & Limitations
- 5 minute user cooldown right after executing the command once.
- 10 minute guild cooldown right after executing the command twice.

#### `/privacy delete user`
- Deletes data of the user who executed this command

##### Cooldowns & Limitations
- **Deleting your data will make you unvalidated and will ask you for validation again**
- 1 day user cooldown right after executing the command once.

#### `/privacy delete guild`
- Deletes data of the guild.

##### Cooldowns & Limitations
- **Deleting guild data requires to kick the bot from it. You can easily add back to the server**
- 1 day guild cooldown right after executing the command once.
- Requires Manage Guild permission from user to execute.