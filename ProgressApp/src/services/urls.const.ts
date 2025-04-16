//START: authengication
export const AUTHENTICATE_REFRESH_TOKEN_URL = () =>
  `authenticate/refresh-token`;
export const AUTHENTICATE_LOGIN_URL = () => `authenticate/login`;
export const AUTHENTICATE_REVOKE_URL = () => `authenticate/revoke`;

//account
export const ACCOUNT_CHANGE_PASSWORD_URL = () => `account/change-password`;

//users
export const USERS_GET_LIST_URL = () => `users`;
export const USERS_GET_BY_ID_URL = (userId: string) => `users/${userId}`;
export const USERS_CREATE_URL = () => 'users';
export const USERS_UPDATE_URL = () => 'users';
export const USERS_SET_USER_ROLES_URL = (userId: string) =>
  `users/${userId}/roles`;
export const USERS_CHANGE_PASSWORD_URL = () => `users/change-password`;
//END: authengication

//roles
export const ROLES_GET_LIST_URL = () => `roles`;

//limits
export const LIMITS_GET_URL = () => `limits`;

//START: lines
export const LINE_GET_URL = () => `lines`;
export const LINE_GET_CURRENT_LINE_INFO_URL = (lineId: number) =>
  `lines/${encodeURIComponent('' + lineId)}/currentInfo`;
//END: lines

//START: coils
export const COIL_BROWSE_COILS_URL = (lineId: number) =>
  `lines/${encodeURIComponent('' + lineId)}/coils/browse`;
export const COIL_GET_COIL_ORDER_DATA_URL = (lineId: number, coilId: string) =>
  `lines/${encodeURIComponent('' + lineId)}/coils/${encodeURIComponent(
    coilId,
  )}/order-data`;
export const COIL_GET_VPL_COIL_ORDER_DATA_URL = (
  lineId: number,
  coilId: string,
) =>
  `lines/${encodeURIComponent('' + lineId)}/coils/${encodeURIComponent(
    coilId,
  )}/vpl-order-data`;

//version
export const VERSION_GET = () => 'version';
export const VERSION_BUILD_TIME = () => 'version/buildTime';
