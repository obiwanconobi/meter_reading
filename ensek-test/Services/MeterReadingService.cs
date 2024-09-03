using ensek_test.DataAccess.DbContexts;
using ensek_test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace ensek_test.Services
{
    public interface IMeterReadingService
    {
        Task<(int SuccessCount, int FailureCount)> ProcessMeterReadingsAsync(Stream csvStream);
    }

    public class MeterReadingService : IMeterReadingService
    {
        private UnitOfWork context;
        public MeterReadingService()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnitOfWork>();
            context = new UnitOfWork(optionsBuilder.Options);
        }

        public async Task<(int SuccessCount, int FailureCount)> ProcessMeterReadingsAsync(Stream csvStream)
        {
            int successCount = 0;
            int failureCount = 0;
                    
            using (TextFieldParser parser = new TextFieldParser(csvStream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.HasFieldsEnclosedInQuotes = false;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    if (TryParseMeterReading(parser.ReadFields(), out MeterReading meterReading))
                    {
                        if (await IsValidMeterReading(meterReading))
                        {
                            context.meter_readings.Add(meterReading);
                            successCount++;
                        }
                        else
                        {
                            failureCount++;
                        }
                    }
                    else
                    {
                        failureCount++;
                    }
                }
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }


            return (successCount, failureCount);
            
        }

        public async Task<bool> IsValidMeterReading(MeterReading meterReading)
        {
            
            var accountExists = await context.accounts.AnyAsync(x => x.AccountId == meterReading.AccountId);
            if (!accountExists)
            {
                return false;
            }

            if (!int.TryParse(meterReading.MeterReadValue, out int meterReadValue) || meterReadValue < 0 || meterReadValue > 99999)
            {
                return false;
            }

            var duplicateExists = await context.meter_readings.AnyAsync(m =>
               m.AccountId == meterReading.AccountId &&
               m.MeterReadingDateTime == meterReading.MeterReadingDateTime);

            return !duplicateExists;
        }

        public bool TryParseMeterReading(dynamic record, out MeterReading meterReading)
        {
            meterReading = null;

            try
            {
                if (int.TryParse(record[0], out int accountId) &&
                DateTime.TryParse(record[1], out DateTime result) &&
                !string.IsNullOrEmpty(record[2]))
                {

                    meterReading = new MeterReading
                    {
                        Id = Guid.NewGuid(),
                        AccountId = accountId,
                        MeterReadingDateTime = DateTime.Parse(record[1]),
                        MeterReadValue = record[2]
                    };
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
