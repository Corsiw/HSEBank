using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Export
{
    public class CsvFileVisitor : IFileExporter, IVisitor
    {
        private readonly List<object> _items = [];
        
        public string FileExtension => ".csv";

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
            await using StreamWriter writer = new(filePath);
            await using CsvWriter csv = new(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

            if (_items.Count > 0)
            {
                await csv.WriteRecordsAsync(_items);
            }
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