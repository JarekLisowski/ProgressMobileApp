package com.example.progressbt;

import android.Manifest;
import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.util.Log;

import androidx.activity.ComponentActivity;
import androidx.core.app.ActivityCompat;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Set;
import java.util.UUID;

public class OLDBluetoothHandler {

    private static final String TAG = "BluetoothHandler";
    private static final UUID MY_UUID_INSECURE = UUID.fromString("8ce255c0-200a-11e0-ac64-0800200c9a66"); // Przykładowy UUID, zmień na UUID serwera

    private BluetoothAdapter bluetoothAdapter;
    private Context context;
    private BluetoothDevice targetDevice;
    private BluetoothSocket socket;
    private InputStream inputStream;
    private OutputStream outputStream;

    public OLDBluetoothHandler(Context context) {
        this.context = context;
        bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        //BluetoothManager.getAdapter();
    }

    // Sprawdź, czy Bluetooth jest obsługiwany i włączony
    public boolean isBluetoothEnabled() {
        return bluetoothAdapter != null && bluetoothAdapter.isEnabled();


    }

    // Włącz Bluetooth (jeśli nie jest włączony)
    @SuppressLint("MissingPermission")
    public boolean enableBluetooth() {
        if (bluetoothAdapter != null && !bluetoothAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            context.startActivity(enableBtIntent); // Musisz to wywołać z Activity, a nie z Contextu Aplikacji
            return true; // Zakładamy, że użytkownik odpowie na prośbę
        }
        return true; // Bluetooth już włączony lub brak adaptera
    }

    // Pobierz listę sparowanych urządzeń
    @SuppressLint("MissingPermission")
    public Set<BluetoothDevice> getPairedDevices() {
        if (bluetoothAdapter != null) {
            return bluetoothAdapter.getBondedDevices();
        }
        return null;
    }

    // Rozpocznij wyszukiwanie dostępnych urządzeń
    @SuppressLint("MissingPermission")
    public void startDiscovery() {
        if (bluetoothAdapter != null && bluetoothAdapter.isDiscovering()) {
            bluetoothAdapter.cancelDiscovery();
        }
        if (ActivityCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_SCAN) != PackageManager.PERMISSION_GRANTED) {
            Log.e(TAG, "Brak uprawnień BLUETOOTH_SCAN");
            return;
        }
        bluetoothAdapter.startDiscovery();
    }

    // Zatrzymaj wyszukiwanie urządzeń
    @SuppressLint("MissingPermission")
    public void cancelDiscovery() {
        if (bluetoothAdapter != null && bluetoothAdapter.isDiscovering()) {
            bluetoothAdapter.cancelDiscovery();
        }
    }

    // BroadcastReceiver do obsługi zdarzeń Bluetooth (np. znalezienie urządzenia)
    private final BroadcastReceiver receiver = new BroadcastReceiver() {
        @SuppressLint("MissingPermission")
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                // Znaleziono urządzenie
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                String deviceName = device.getName();
                String deviceAddress = device.getAddress(); // MAC address
                Log.d(TAG, "Znaleziono urządzenie: " + deviceName + " (" + deviceAddress + ")");
                // Tutaj możesz dodać logikę do obsługi znalezionego urządzenia, np. dodanie do listy
            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                // Wyszukiwanie zakończone
                Log.d(TAG, "Wyszukiwanie zakończone.");
                // Tutaj możesz zaktualizować UI lub wykonać inne akcje po zakończeniu wyszukiwania
            }
        }
    };

    // Zarejestruj BroadcastReceiver w Activity, która będzie używać wyszukiwania
    public void registerReceiver(ComponentActivity activity) {
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        activity.registerReceiver(receiver, filter);
        IntentFilter filter2 = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
        activity.registerReceiver(receiver, filter2);
    }

    // Wyrejestruj BroadcastReceiver w Activity, gdy nie jest już potrzebny (np. w onDestroy)
    public void unregisterReceiver(ComponentActivity activity) {
        try {
            activity.unregisterReceiver(receiver);
        } catch (IllegalArgumentException e) {
            // Receiver not registered
        }
    }

    // Połącz się z urządzeniem o podanym adresie MAC
    @SuppressLint("MissingPermission")
    public boolean connectToDevice(BluetoothDevice deviceToConnect) {
        if (bluetoothAdapter == null) {
            return false;
        }
        try {
            BluetoothDevice device = bluetoothAdapter.getRemoteDevice(deviceToConnect.getAddress());
            if (ActivityCompat.checkSelfPermission(context, Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) {
                Log.e(TAG, "Brak uprawnień BLUETOOTH_CONNECT");
                return false;
            }
            socket = device.createInsecureRfcommSocketToServiceRecord(MY_UUID_INSECURE);
            //bluetoothAdapter.cancelDiscovery(); // Zawsze zatrzymuj wyszukiwanie przed połączeniem

            socket.connect();
            inputStream = socket.getInputStream();
            outputStream = socket.getOutputStream();
            Log.d(TAG, "Połączono z: " + device.getAddress());
            return true;
        } catch (IOException e) {
            Log.e(TAG, "Błąd połączenia: " + e.getMessage());
            closeConnection();
            return false;
        }
    }

    // Wyślij dane do podłączonego urządzenia
    public void write(byte[] bytes) {
        try {
            if (outputStream != null) {
                outputStream.write(bytes);
            } else {
                Log.e(TAG, "OutputStream jest null.");
            }
        } catch (IOException e) {
            Log.e(TAG, "Błąd wysyłania danych: " + e.getMessage());
            closeConnection();
        }
    }

    // Odczytaj dane z podłączonego urządzenia (należy obsłużyć w osobnym wątku)
    public void read() {
        final int BUFFER_SIZE = 1024;
        byte[] buffer = new byte[BUFFER_SIZE];
        int bytes;

        while (socket != null && socket.isConnected()) {
            try {
                if (inputStream != null) {
                    bytes = inputStream.read(buffer);
                    if (bytes > 0) {
                        final String receivedData = new String(buffer, 0, bytes);
                        Log.d(TAG, "Odebrano: " + receivedData);
                        // Tutaj możesz przekazać odebrane dane do UI lub innej logiki
                    }
                }
            } catch (IOException e) {
                Log.e(TAG, "Błąd odczytu danych: " + e.getMessage());
                closeConnection();
                break;
            }
        }
    }

    // Zamknij połączenie
    public void closeConnection() {
        try {
            if (inputStream != null) inputStream.close();
            if (outputStream != null) outputStream.close();
            if (socket != null && socket.isConnected()) socket.close();
            inputStream = null;
            outputStream = null;
            socket = null;
            Log.d(TAG, "Połączenie zamknięte.");
        } catch (IOException e) {
            Log.e(TAG, "Błąd zamykania połączenia: " + e.getMessage());
        }
    }
}