import { useVuelidate } from '@vuelidate/core'
import { required, email as isEmail } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useForgotValidation(state: { email: Ref<string> }) {
  const rules = { email: { required, email: isEmail } }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useForgotValidation
