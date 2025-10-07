export const environment = {
  production: false,
  url: 'http://192.168.33.5',
  getBackendApiUrl() {
    return `${this.url}:5085/`;
  },
  getBackendUrl() {
    return `${this.url}:4200/`;
  }
};