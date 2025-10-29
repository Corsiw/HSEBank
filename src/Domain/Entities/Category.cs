using Domain.Enums;

namespace Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public MoneyType Type { get; private set; }
        public string Name { get; private set; }


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
    }
}