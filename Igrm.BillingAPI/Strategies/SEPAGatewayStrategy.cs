using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Igrm.BillingAPI.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Igrm.BillingAPI.Strategies
{
    public class SEPAGatewayStrategy : GatewayStrategyBase, IGatewayStrategy
    {
        public SEPAGatewayStrategy(IBillingConfigurationService billingConfigurationService, ILogger logger,  IOrderRepository orderRepository, IMemoryCache cache) : base(billingConfigurationService, logger, orderRepository, cache)  { }

        public string GetPaymentTo()
        {
            return _billingConfigurationService.GetGatewaySpecificConfig(Gateway.SEPA).CompanyIBAN;
        }

        public async Task<ICollection<SettlementAllocationDTO>> GetSettlementsAsync(string? orderNumber, string paymentTo)
        {
            var result = new List<SettlementAllocationDTO>();
            var xmlFiles = GetFileList(EnsureSourceDirectory());

            _logger.LogInformation($"ISO 20022 XML camt.053 ver 2 Account statement files found: {xmlFiles.Count}");

            foreach (var xmlFile in xmlFiles)
            {
                var xdoc = LoadXDocument(xmlFile);
                XNamespace ns = "urn:iso:std:iso:20022:tech:xsd:camt.053.001.02";
                var dto =  xdoc.Descendants(ns + "Ntry")?
                               .Where(x => x?.Element(ns + "CdtDbtInd")?.Value == "DBIT"
                                           && !String.IsNullOrEmpty(orderNumber)
                                           && (x?.Element(ns + "AcctSvcrRef")?.Value?.Contains(orderNumber) ?? false))
                               .Select(x => new SettlementAllocationDTO()
                               {
                                   PaymentDate = Convert.ToDateTime(x?.Element(ns + "BookgDt")?.Element(ns + "Dt")?.Value),
                                   PaidAmount = Convert.ToDecimal(x?.Element(ns + "Amt")?.Value),
                                   Description = x?.Element(ns + "AcctSvcrRef")?.Value,
                                   PaymentFrom = x?.Descendants(ns + "CdtrAcct").FirstOrDefault()?.Element(ns + "Id")?.Element(ns + "IBAN")?.Value,
                                   TxReference = x?.Element(ns + "NtryRef")?.Value
                               });

                _logger.LogInformation($"Number of settlements found for {orderNumber} in file {xmlFile} : {dto?.Count() ?? 0} ");
                
                if (dto is not null) { result.AddRange(dto); }
            }

            return await Task.FromResult(result);
        }

        protected virtual string EnsureSourceDirectory()
        {
            var sourceDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), (string)_billingConfigurationService.GetGatewaySpecificConfig(Gateway.SEPA).XmlFilePath);
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
                _logger.LogInformation($"Directory for gateway xml files SEPA created: {sourceDirectory}");
            }

            return sourceDirectory;
        }

        protected virtual List<string> GetFileList(string sourceDirectory)
        {
            return Directory.EnumerateFiles(sourceDirectory, "*.xml", SearchOption.AllDirectories).ToList();
        }

        protected virtual XDocument LoadXDocument(string path)
        {
            return XDocument.Load(path);
        }
    }
}
