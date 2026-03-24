namespace Conferenti.Infrastructure.Settings;

public record KeyVaultSettings(bool UseEmulator, string VaultEndPoint, bool BypassKeyVault = false);
