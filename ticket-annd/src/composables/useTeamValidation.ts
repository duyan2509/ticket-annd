import { useVuelidate } from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import type { Ref } from 'vue'

export function useTeamValidation(state: { name: Ref<string> }) {
  const rules = { name: { required } }
  const v$ = useVuelidate(rules, state)
  return { v$ }
}

export default useTeamValidation
