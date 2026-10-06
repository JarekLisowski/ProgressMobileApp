export const environment = {
  production: false,
  url: 'http://192.168.33.2',
  getBackendApiUrl() {
    return 'https://api.progress.ifox.com.pl/';
    return `${this.url}:5085/`;
  },
  getBackendUrl() {
    return `${this.url}:4200/`;
  }
};