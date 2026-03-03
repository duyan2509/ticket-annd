
export const AppRoles = {
  CompanyAdmin: 'CompanyAdmin',
  Agent: 'Agent',
  Customer: 'Customer',
  EmptyUser: 'EmptyUser',
  SupperAdmin: 'SupperAdmin',
} as const

export type AppRole = (typeof AppRoles)[keyof typeof AppRoles]
