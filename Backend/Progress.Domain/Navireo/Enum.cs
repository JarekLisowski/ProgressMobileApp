namespace Progress.Domain.Navireo
{
    /// <summary>
    /// Rezultat generowania tokenu
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// Zalogowano poprawnie
        /// </summary>
        OK = 1,

        /// <summary>
        /// Błąd - niepoprawny login lub hasło
        /// </summary>
        LoginError = 2,

        /// <summary>
        /// Niepoprawny identyfikator urządzenia
        /// </summary>
        DeviceError = 3,

        /// <summary>
        /// Exception podczas logowania użytkownika
        /// </summary>
        Exception = 4,

        /// <summary>
        /// Nieaktualna wersja aplikacji użytkownika
        /// </summary>
        ExpiredVersion = 5
    }

    /// <summary>
    /// Rezultat dodania dokumentu
    /// </summary>
    public enum DocumentUpdateResult
    {
        /// <summary>
        /// Dokument zapisany poprawnie
        /// </summary>
        OK = 1,

        /// <summary>
        /// NIeaktualny token użytkownika
        /// </summary>
        ExpiredToken = 2,

        /// <summary>
        /// Nieaktualna cena pozycji dokumentu
        /// </summary>
        ExpiredProductPrice = 3,

        /// <summary>
        /// Kontrahent zablokowany
        /// </summary>
        CustomerError = 4,

        /// <summary>
        /// Jeden z towarów zablokowany lub nieaktywny
        /// </summary>
        ProductError = 5,

        /// <summary>
        /// Exception podczas zapisu dokumentu
        /// </summary>
        Exception = 6,

        /// <summary>
        /// Niepoprawna numeracja dokumentu
        /// </summary>
        NumberError = 7
    }

    /// <summary>
    /// Base result
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Akcja wykonana poprawnie
        /// </summary>
        OK = 1,

        /// <summary>
        /// Błąd podczas wykonywania akcji
        /// </summary>
        Exception = 3,

        /// <summary>
        /// Akcja wykonana be zbłedów jednak nie zwróciła wyników
        /// </summary>
        NotFound = 0
    }
    
    /// <summary>
    /// Rezultat dodania kontrahenta
    /// </summary>
    public enum BusinessUpdateResult
    {
        /// <summary>
        /// Kontrahent zapisany poprawnie
        /// </summary>
        OK = 1,

        /// <summary>
        /// Nieaktualny token użytkownika
        /// </summary>
        ExpiredToken = 2,

        /// <summary>
        /// Błędny symbol kontrahenta 
        /// </summary>
        InvalidCode = 3, //istnieje już kontrahent o podanym symbolu

        /// <summary>
        /// Błąd podczas zapisu kontrahenta
        /// </summary>
        Exception = 4
    }
}
