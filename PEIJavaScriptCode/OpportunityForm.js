var Opportunity = Opportunity || {}

Opportunity.Form = {
  /**
   * Function to calculate the Estimated Revenue based on Opportunity Type
   * @param {object} executionContext
   */
  OnChangeOpportunityType: function (executionContext) {
    let formContext = executionContext?.getFormContext()
    let opportunityType = formContext
      .getAttribute('pei_opportunitytype')
      ?.getValue()
    switch (opportunityType) {
      case 10000000:
        // Opportunity Type is Fixed Price
        formContext.getControl('estimatedvalue').setDisabled(true) // Disable Estimated value
        break
      case 10000001:
        // Opportunity Type is Variable Price
        formContext.getControl('estimatedvalue').setDisabled(false) // Enabled the field as value is changed to Variable Price
        let unitPrice = formContext.getAttribute('pei_unitprice')?.getValue() // get Unit price value
        let totalUnits = formContext.getAttribute('pei_totalunits')?.getValue() // get total units
        let discount = formContext.getAttribute('pei_discount')?.getValue() // get discpount value
        let estimatedRevenue = unitPrice * totalUnits - discount // calculating the revenue
        formContext.getAttribute('estimatedvalue').setValue(estimatedRevenue) // setting revenue value.
        break
    }
  },
  /**
   * Function to set the Estimated Value field as disabled or enabled based on Opportunity Type
   * This function is called on form load to set the initial state of the field
   * @param {object} executionContext
   */
  OnLoad: function (executionContext) {
    let formContext = executionContext?.getFormContext()
    let opportunityType = formContext
      .getAttribute('pei_opportunitytype')
      ?.getValue()

    if (opportunityType === 10000000) {
      // Opportunity Type is Fixed Price
      formContext.getControl('estimatedvalue').setDisabled(true) // Disable Estimated value
    } else {
      formContext.getControl('estimatedvalue').setDisabled(false) // Enabled the field as value is not to Fixed Price
    }
  },
}

if (typeof module !== 'undefined' && module.exports) {
  module.exports = Opportunity
}
