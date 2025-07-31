package com.example.progressbt

import android.Manifest
import android.annotation.SuppressLint
import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothDevice
import android.bluetooth.BluetoothSocket
import android.content.Intent
import android.content.pm.PackageManager
import android.util.Log
import androidx.activity.ComponentActivity
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.ContextCompat
import java.io.IOException
import java.io.OutputStream
import java.nio.charset.Charset
import java.util.UUID

class BluetoothHandler(private val activity: ComponentActivity) {

    private val bluetoothAdapter: BluetoothAdapter? by lazy { BluetoothAdapter.getDefaultAdapter() }
    private var bluetoothSocket: BluetoothSocket? = null
    private var outputStream: OutputStream? = null
    private val deviceName = "WOOSIM"
    private val sppUuid = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB")

    private lateinit var enableBluetoothLauncher: ActivityResultLauncher<Intent>
    private lateinit var requestBluetoothConnectPermissionLauncher: ActivityResultLauncher<String>

    init {
        setupBluetoothLaunchers()
    }

    private fun setupBluetoothLaunchers() {
        enableBluetoothLauncher = activity.registerForActivityResult(ActivityResultContracts.StartActivityForResult()) { result ->
            if (result.resultCode == Activity.RESULT_OK) {
                Log.i("Bluetooth", "Bluetooth was enabled.")
            } else {
                Log.w("Bluetooth", "User denied enabling Bluetooth.")
            }
        }

        requestBluetoothConnectPermissionLauncher =
            activity.registerForActivityResult(ActivityResultContracts.RequestPermission()) { isGranted: Boolean ->
                if (isGranted) {
                    Log.i("Bluetooth", "Bluetooth connect permission granted.")
                } else {
                    Log.w("Bluetooth", "Bluetooth connect permission denied.")
                }
            }
    }

    fun checkBluetoothPermissions() {
        if (bluetoothAdapter == null) {
            Log.e("Bluetooth", "Device does not support Bluetooth.")
            return
        }

        if (!bluetoothAdapter!!.isEnabled) {
            val enableBtIntent = Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE)
            enableBluetoothLauncher.launch(enableBtIntent)
            return
        }

        if (ContextCompat.checkSelfPermission(activity, Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) {
            requestBluetoothConnectPermissionLauncher.launch(Manifest.permission.BLUETOOTH_CONNECT)
            return
        }
    }

    @SuppressLint("MissingPermission")
    private fun findAndConnectDevice(): String {
        if (ContextCompat.checkSelfPermission(activity, Manifest.permission.BLUETOOTH_CONNECT) != PackageManager.PERMISSION_GRANTED) {
            Log.e("Bluetooth", "Missing BLUETOOTH_CONNECT permission.")
            return "Missing BLUETOOTH_CONNECT permission."
        }

        val pairedDevices: Set<BluetoothDevice>? = bluetoothAdapter?.bondedDevices
        pairedDevices?.forEach { device ->
            if (device.name == deviceName) {
                Log.i("Bluetooth", "Found device: ${device.name} - ${device.address}")
                return connectToDevice(device)
            }
        }
        Log.w("Bluetooth", "Paired device not found: $deviceName")
        return "Paired device not found: $deviceName"
    }

    @SuppressLint("MissingPermission")
    private fun connectToDevice(device: BluetoothDevice): String {
        try {
            bluetoothSocket = device.createRfcommSocketToServiceRecord(sppUuid)
            bluetoothSocket?.connect()
            Log.i("Bluetooth", "Connected to device: ${device.name}")
            outputStream = bluetoothSocket?.outputStream
            return "OK"
        } catch (e: IOException) {
            Log.e("Bluetooth", "Connection error: ${e.localizedMessage}")
            closeConnection()
            return "Connection error: ${e.localizedMessage}"
        }
    }

    private fun sendData(data: String) {
        try {
            outputStream?.write(data.toByteArray())
            Log.i("Bluetooth", "Data sent: $data")
        } catch (e: IOException) {
            Log.e("Bluetooth", "Error sending data: ${e.localizedMessage}")
            closeConnection()
        }
    }

    private fun sendData(data: ByteArray) {
        try {
            outputStream?.write(data)
            Log.i("Bluetooth", "Data sent: $data")
        } catch (e: IOException) {
            Log.e("Bluetooth", "Error sending data: ${e.localizedMessage}")
            closeConnection()
        }
    }

    private fun closeConnection() {
        try {
            Thread.sleep(500)
            outputStream?.close()
            bluetoothSocket?.close()
            Log.i("Bluetooth", "Connection closed.")
        } catch (e: IOException) {
            Log.e("Bluetooth", "Error closing connection: ${e.localizedMessage}")
        } finally {
            outputStream = null
            bluetoothSocket = null
        }
    }

    fun setupPrinter() {
        //Ustawia szeroki zestaw znaków (65 zn/linia)
        var data = byteArrayOf(27, 33, 0)
        sendData(data)
        //Ustawia polską stronę kodową CP852
        data = byteArrayOf(27, 116, 12)
        sendData(data)
    }

    fun print(text: String) {
        val connectionResult = findAndConnectDevice()
        if (connectionResult == "OK") {
            setupPrinter();
            val cp852Charset = Charset.forName("CP852")
            val dataToPrint = text.toByteArray(cp852Charset)
            sendData(dataToPrint)
            closeConnection()
        } else {
            Log.e("Bluetooth", "Could not print. Connection failed: $connectionResult")
        }
    }

    fun print(data: ByteArray) {
        val connectionResult = findAndConnectDevice()
        if (connectionResult == "OK") {
            setupPrinter();
            sendData(data)
            closeConnection()
        } else {
            Log.e("Bluetooth", "Could not print. Connection failed: $connectionResult")
        }
    }
}