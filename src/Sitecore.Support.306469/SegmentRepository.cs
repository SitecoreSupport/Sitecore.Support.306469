using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Sitecore.Abstractions;
using Sitecore.Services.Core;
using Sitecore.ListManagement.Services.Model;
using Sitecore.Diagnostics;
using Sitecore.ListManagement.Services.Exceptions;
using System.Runtime.CompilerServices;
using Sitecore.Marketing.Definitions.Segments;
using Sitecore.Data.Items;
using Sitecore.Marketing;
using Sitecore.Marketing.Rules;
using Sitecore.Marketing.Segmentation.RuleXmlConverter;
using System.Xml.Linq;
using Sitecore.Marketing.Definitions;

namespace Sitecore.Support.ListManagement.Services.Repositories
{
  public class SegmentRepository : Sitecore.ListManagement.Services.Repositories.SegmentRepository, IRepository<SegmentModel>
  {
    private readonly BaseLog _log;
    private readonly IRuleXmlConverter _ruleXmlConverter;
    private readonly IDefinitionManager<ISegmentDefinition> _segmentDefinitionManager;
    public SegmentRepository(Sitecore.Marketing.Definitions.IDefinitionManager<Sitecore.Marketing.Definitions.Segments.ISegmentDefinition> segmentDefinitionManager, Sitecore.Marketing.Segmentation.RuleXmlConverter.IRuleXmlConverter ruleXmlConverter, CultureInfo defaultCulture, BaseLog log) : base(segmentDefinitionManager, ruleXmlConverter, defaultCulture, log)
    {
      this._log = log;
      this._ruleXmlConverter = ruleXmlConverter;
      this._segmentDefinitionManager = segmentDefinitionManager;
    }

   void IRepository<SegmentModel>.Add(SegmentModel entity)
    {
      Assert.ArgumentNotNull(entity, "entity");
      AssertId(entity.Id);
      Guid guid = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid() : Guid.Parse(entity.Id);
      SegmentDefinition segmentDefinition = new SegmentDefinition(guid, ItemUtil.ProposeValidItemName(FormattableString.Invariant(FormattableStringFactory.Create("{0}-{1}", entity.Name, guid))), Context.Language.CultureInfo, entity.Name, DateUtil.ToUniversalTime(DateUtil.IsoDateToDateTime(entity.Created, DateTime.UtcNow)), entity.CreatedBy ?? Context.User.Name)
      {
        Description = entity.Description
      };
      segmentDefinition.Rules.CopyFrom(CreateRulesFromXml(entity.RulesXml));
      _segmentDefinitionManager.SaveAsync(segmentDefinition, true);
    }

    private IEnumerable<Rule> CreateRulesFromXml(string xmlData)
    {
      if (!string.IsNullOrEmpty(xmlData))
      {
        return _ruleXmlConverter.CreateRules(XDocument.Parse(xmlData));
      }
      return Enumerable.Empty<Rule>();
    }

    private void AssertId(string id)
    {
      Guid g;
      if (!Guid.TryParse(id, out g))
      {
        string message = FormattableString.Invariant(FormattableStringFactory.Create("{0} is not a valid Guid.", id));
        UnprocessableEntityException ex = new UnprocessableEntityException(message);
        _log.Error(message, ex, this);
        throw ex;
      }
    }
  }
}