namespace BlazorBudgetWebWindow.Model {

    using System;

    public class BudgetAllocationItem {

        public Decimal Amount { get; set; }

        public Boolean IsSavingsItem { get; set; }

        public Decimal MaxValue { get; set; }

        public String Name { get; set; }

        public Single WidthPercent { get; set; }

        public BudgetAllocationItem() {
        }
    }
}
