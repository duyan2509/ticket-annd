import { useVuelidate } from '@vuelidate/core'
import { required, email as isEmail, minLength } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useRegisterValidation(state: { email: Ref<string>; password: Ref<string> }) {
  const rules = {
    email: { required, email: isEmail },
    password: { required, minLength: minLength(6) },
  }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useRegisterValidation
