using Sitecore.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.ListManagement.Services.Repositories;
using Sitecore.ListManagement.Services.Model;
using Sitecore.Marketing.Definitions;
using Sitecore.Data.Managers;
using Sitecore.Marketing.Definitions.Segments;
using Sitecore.Marketing.Segmentation.RuleXmlConverter;

namespace Sitecore.Support
{
  public class OverrideSegmentRepositoryConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      var descriptor = serviceCollection.FirstOrDefault(d => d.ServiceType == typeof(Sitecore.ListManagement.Services.Repositories.SegmentRepository));
      if (descriptor != null)
      {
        serviceCollection.Remove(descriptor);
      }
      serviceCollection.AddTransient<IFetchRepository<SegmentModel>, Sitecore.Support.ListManagement.Services.Repositories.SegmentRepository>((IServiceProvider s) => new Sitecore.Support.ListManagement.Services.Repositories.SegmentRepository(s.GetService<IDefinitionManager<ISegmentDefinition>>(), s.GetService<IRuleXmlConverter>(), LanguageManager.DefaultLanguage.CultureInfo, s.GetService<BaseLog>()));
    }
  }
}