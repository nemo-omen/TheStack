
using Markdig;
using Markdig.Parsers;

namespace TheStack.Domain;

public sealed class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "Untitled";
    public string? Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Due { get; set; }
    public bool Done { get; set; }
    public int Priority { get; set; }
    public Guid? ParentId { get; set; }
    public Node? Parent { get; set; }
    public List<Node> Children { get; set; } = new();
    
    private Node() {}
    
    public Node(string name, string? content, DateTime? due, Guid? parentId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Content = content;
        Created = DateTime.UtcNow;
        Due = due;
        ParentId = parentId;
    }

    public NodeType Type()
    {
        return Children.Count > 0 ? NodeType.Stack : NodeType.Task;
    }

    public string HtmlContent()
    {
        if(string.IsNullOrWhiteSpace(Content)) return "";
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
            var parsed = Markdown.Parse(Content, pipeline);
            return parsed.ToString()??"";
    }

    public void AddChild(Node child)
    {
        Children.Add(child);
        child.SetParent(this);
        child.Priority = Children.Count;
    }
    
    public void RemoveChild(Guid id)
    {
        var child = Children.FirstOrDefault(c => c.Id == id);
        if (child != null)
        {
            child.RemoveParent();
            child.Priority = 0;
        }
        Children.RemoveAll(c => c.Id == id);
    }
    
    public void SetParent(Node parent)
    {
        ParentId = parent.Id;
        Parent = parent;
    }
    
    public void RemoveParent()
    {
        ParentId = null!;
        Parent = null!;
    }

    private sealed class IdNameContentEqualityComparer : IEqualityComparer<Node>
    {
        public bool Equals(Node? x, Node? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id) && x.Name == y.Name && x.Content == y.Content;
        }

        public int GetHashCode(Node obj)
        {
            return HashCode.Combine(obj.Id, obj.Name, obj.Content);
        }
    }
    private bool Equals(Node other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Node other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Content);
    }

    public static IEqualityComparer<Node> IdNameContentComparer { get; } = new IdNameContentEqualityComparer();
}