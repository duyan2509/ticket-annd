import { useVuelidate } from '@vuelidate/core'
import { required, email as isEmail } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useInviteValidation(state: { email: Ref<string>; role: Ref<string> }) {
  const rules = { email: { required, email: isEmail }, role: { required } }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useInviteValidation
