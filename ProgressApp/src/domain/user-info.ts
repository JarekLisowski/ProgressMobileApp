import { JwtPayload } from 'jwt-decode';
import { AuthLoginInfoDto } from './generated/apimodel';
//import { AuthLoginInfoDto } from './authLoginInfo';

export const SUPERADMIN_ROLE = 'SUPERADMIN_ROLE';
export const ADMIN_ROLE = 'ADMIN_ROLE';
export const isSuperAdmin = (roles?: string[]): boolean => {
  return (
    roles !== undefined &&
    roles.find((x) => x === SUPERADMIN_ROLE) !== undefined
  );
};

export class UserInfo implements JwtPayload {
  readonly iss?: string;
  readonly sub?: string;
  readonly aud?: string[] | string;
  readonly exp?: number;
  readonly nbf?: number;
  readonly iat?: number;
  readonly jti?: string;

  readonly roles: string[];
  readonly name: string;
  readonly email: string;
  readonly firstName: string;
  readonly lastName: string;
  readonly userId: string;

  readonly authLoginInfo: AuthLoginInfoDto;

  constructor(currentUser: any, authLoginInfoDto: AuthLoginInfoDto) {
    Object.assign(this, currentUser);

    this.authLoginInfo = authLoginInfoDto;

    const r = currentUser['roles'] ?? [];
    this.roles = typeof r === 'string' ? [r] : r;

    this.name = currentUser['unique_name'] ?? '';
    this.email = currentUser['email'] ?? '';
    this.firstName = currentUser['name'] ?? '';
    this.lastName = currentUser['family_name'] ?? '';
    this.userId = currentUser['sub'] ?? '';
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
  get isAdmin(): boolean {
    return this.hasRole(SUPERADMIN_ROLE) || this.hasRole(ADMIN_ROLE);
  }
  hasRole(roleName: string): boolean {
    return (
      this.roles.find((x) => x.toLowerCase() === roleName.toLowerCase()) !==
      undefined
    );
  }
}
