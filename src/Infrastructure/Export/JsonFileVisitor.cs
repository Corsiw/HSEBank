using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Export
{
    public class JsonFileVisitor : IFileExporter, IVisitor
    {
        private readonly List<object> _items = [];

        public string FileExtension => ".json";

        public void BeginExport()
        {
            _items.Clear();
        }

        public void ExportItem(IVisitable item)
        {
            item.Accept(this);
        }

        public async Task EndExport(string filePath)
        {
            string json = JsonSerializer.Serialize(_items, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
        }

        public bool CanExport(Type type, string ext)
        {
            throw new NotImplementedException();
        }


        public void Visit(BankAccount account)
        {
            _items.Add(account);
        }

        public void Visit(Category category)
        {
            _items.Add(category);
        }

        public void Visit(Operation operation)
        {
            _items.Add(operation);
        }
    }
}