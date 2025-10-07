export const environment = {
  production: true,
  url: 'http://api.progress.ifox.com.pl',
  getBackendApiUrl() {
    return this.url;
  },
  getBackendUrl() {
    return this.url;
  }
};