import { useVuelidate } from '@vuelidate/core'
import { required, minValue } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useSlaRuleValidation(state: { name: Ref<string>; firstResponseMinutes: Ref<number | null>; resolutionMinutes: Ref<number | null> }) {
  const rules = {
    name: { required },
    firstResponseMinutes: { required, minValue: minValue(0) },
    resolutionMinutes: { required, minValue: minValue(0) },
  }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useSlaRuleValidation
