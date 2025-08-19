const {
  initFormContext,
  buildExecutionContext,
} = require('./test-utils/d365TestHarness.js')
const Opportunity = require('./OpportunityForm.js')
describe('Opportunity.Form JS Tests', () => {
  describe('OnChangeOpportunityType', () => {
    test('Disables estimatedvalue for Fixed Price (10000000)', () => {
      const formContext = initFormContext({
        pei_opportunitytype: 10000000,
        estimatedvalue: 0,
      })

      // Mock control
      formContext.getControl = jest.fn().mockReturnValue({
        setDisabled: jest.fn(),
      })

      const ctx = buildExecutionContext(formContext)

      Opportunity.Form.OnChangeOpportunityType(ctx)

      expect(formContext.getControl).toHaveBeenCalledWith('estimatedvalue')
      expect(
        formContext.getControl('estimatedvalue').setDisabled
      ).toHaveBeenCalledWith(true)
    })

    test('Enables estimatedvalue and calculates revenue for Variable Price (10000001)', () => {
      const formContext = initFormContext({
        pei_opportunitytype: 10000001,
        pei_unitprice: 200,
        pei_totalunits: 5,
        pei_discount: 100,
        estimatedvalue: 0,
      })

      // Mock control
      formContext.getControl = jest.fn().mockReturnValue({
        setDisabled: jest.fn(),
      })

      const ctx = buildExecutionContext(formContext)

      Opportunity.Form.OnChangeOpportunityType(ctx)

      // Should enable estimatedvalue
      expect(
        formContext.getControl('estimatedvalue').setDisabled
      ).toHaveBeenCalledWith(false)

      // Should calculate revenue: (200 * 5) - 100 = 900
      expect(formContext.getAttribute('estimatedvalue').getValue()).toBe(900)
    })
  })

  describe('OnLoad', () => {
    test('Disables estimatedvalue if Fixed Price on load', () => {
      const formContext = initFormContext({
        pei_opportunitytype: 10000000,
        estimatedvalue: 0,
      })

      formContext.getControl = jest.fn().mockReturnValue({
        setDisabled: jest.fn(),
      })

      const ctx = buildExecutionContext(formContext)

      Opportunity.Form.OnLoad(ctx)

      expect(
        formContext.getControl('estimatedvalue').setDisabled
      ).toHaveBeenCalledWith(true)
    })

    test('Enables estimatedvalue if NOT Fixed Price on load', () => {
      const formContext = initFormContext({
        pei_opportunitytype: 10000001,
        estimatedvalue: 0,
      })

      formContext.getControl = jest.fn().mockReturnValue({
        setDisabled: jest.fn(),
      })

      const ctx = buildExecutionContext(formContext)

      Opportunity.Form.OnLoad(ctx)

      expect(
        formContext.getControl('estimatedvalue').setDisabled
      ).toHaveBeenCalledWith(false)
    })
  })
})
