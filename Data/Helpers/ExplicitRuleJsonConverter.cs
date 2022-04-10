using Sue3.SUM.Model.Components.Descriptive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sue3.Web.Blazor.Data
{
    public class ExplicitRuleJsonConverter : DerivedTypeJsonConverter<Rule>
    {
        protected override Type NameToType(string typeName)
        {
            return typeName switch
            {
                nameof(ExplicitRule) => typeof(ExplicitRule),
                nameof(Rule) => typeof(RelationshipRule)
            };
        }

        protected override string TypeToName(Type type)
        {
            if (type == typeof(ExplicitRule)) return nameof(ExplicitRule);
            return nameof(Rule);
        }
    }
}
