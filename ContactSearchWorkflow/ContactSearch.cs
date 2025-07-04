using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace ContactSearchWorkflow
{
    public class ContactSearch : CodeActivity
    {
        [Input("Search Value 1")]
        public InArgument<string> SearchValue1 { get; set; }

        [Input("First name")]
        public InArgument<string> FirstName { get; set; }

        [Input("Search Value 2")]
        public InArgument<string> SearchValue2 { get; set; }

        [Input("Last name")]
        public InArgument<string> LastName { get; set; }

        [Output("Status")]
        public OutArgument<int> Status { get; set; }

        [Output("Found Contact")]
        public OutArgument<EntityReference> FoundContact { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var value1 = SearchValue1.Get(context);
            var firstNameField = FirstName.Get(context);
            var value2 = SearchValue2.Get(context);
            var lastNameField = LastName.Get(context);

            if (string.IsNullOrEmpty(firstNameField) || string.IsNullOrEmpty(lastNameField))
                throw new InvalidWorkflowException("Field names cannot be null or empty");

            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(null);

            var query = new QueryExpression("Contact")
            {
                ColumnSet = new ColumnSet("Id"),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression(firstNameField, ConditionOperator.Equal, value1),
                        new ConditionExpression(lastNameField, ConditionOperator.Equal, value2)
                    }
                }
            };

            var result = service.RetrieveMultiple(query);

            if (result.Entities.Count == 0)
            {
                Status.Set(context, 2);
            }
            else if (result.Entities.Count == 1)
            {
                var contact = result.Entities[0];
                Status.Set(context, 1);
                FoundContact.Set(context, contact.ToEntityReference());
            }
            else
            {
                Status.Set(context, 3);
            }
        }
    }
}
