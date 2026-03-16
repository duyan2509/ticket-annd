import { useVuelidate } from '@vuelidate/core'
import { required, minLength } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useCreateCompanyValidation(state: { name: Ref<string> }) {
  const rules = { name: { required, minLength: minLength(2) } }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useCreateCompanyValidation
