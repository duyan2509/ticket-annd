import { useVuelidate } from '@vuelidate/core'
import { required, minLength } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useResetValidation(state: { newPassword: Ref<string> }) {
  const rules = { newPassword: { required, minLength: minLength(6) } }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useResetValidation
