using System;

namespace Oty.Interactivity.Entities;

public readonly struct SelectBoxItem
{
    public SelectBoxItem(string id, string label)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id cannot be null or whitespace", nameof(id));
        }

        this.SelectBoxItemId = id.Length > 100 ? id[..100] : id;

        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Label cannot be null or whitespace", nameof(label));
        }
        this.SelectBoxItemLabel = label.Length > 100 ? $"{label[..97]}..." : label;
    }

    public string SelectBoxItemId { get; }

    public string SelectBoxItemLabel { get; }
}