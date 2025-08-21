const {
  initFormContext,
  buildExecutionContext,
} = require('./test-utils/d365TestHarness.js')
const Opportunity = require('./OpportunityForm.js')
/**
 * Opportunity Form Tests
 * These tests cover the JavaScript functionality of the Opportunity form.
 * They validate the behavior of the Opportunity Type field and the Estimated Value field.
 * The tests ensure that the Estimated Value field is disabled when the Opportunity Type is set to Fixed
 * Price and enabled when set to Variable Price.
 * They also check that the Estimated Value is calculated correctly based on the Unit Price, Total Units, and Discount
 * when the Opportunity Type is Variable Price.
 * Each test case initializes the form context and execution context to provide a consistent environment for testing.
 * The tests log their actions and expectations to the console for easier debugging and understanding of the test flow.
 */
describe('Opportunity.Form JS Tests', () => {
  describe('OnChangeOpportunityType', () => {
    test('Disables estimatedvalue for Fixed Price (10000000)', () => {
      const formContext = initFormContext({
        pei_opportunitytype: { type: 'optionset', value: 10000000 },
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
        pei_opportunitytype: { type: 'optionset', value: 10000001 },
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
        pei_opportunitytype: { type: 'optionset', value: 10000000 },
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
        pei_opportunitytype: { type: 'optionset', value: 10000001 },
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
