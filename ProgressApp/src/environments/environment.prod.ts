export interface IEnvironment {
  production: boolean;
  url: string;
  getBackendApiUrl(): string;
  getBackendUrl(): string;
}

export const environment : IEnvironment = {
  production: true,
  url: 'http://api.progress.ifox.com.pl/',
  getBackendApiUrl() {
    return this.url;
  },
  getBackendUrl() {
    return this.url;
  }
};