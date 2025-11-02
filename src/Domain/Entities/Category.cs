using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Category :  IVisitable
    {
        public Guid Id { get; private set; }
        public MoneyType Type { get; private set; }
        public string Name { get; private set; }

#pragma warning disable CS8618 // For EFCore correct mapping
        private Category() { }
#pragma warning restore CS0168 // Re-enable the warning

        public Category(MoneyType type, string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(name));
            }

            Id = id ?? Guid.NewGuid();
            Type = type;
            Name = name;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(newName));
            }

            Name = newName;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}