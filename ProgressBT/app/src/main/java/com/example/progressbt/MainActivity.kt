package com.example.progressbt

import android.Manifest
import android.annotation.SuppressLint
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothDevice
import android.bluetooth.BluetoothSocket
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.core.content.ContextCompat
import androidx.media3.common.util.Util.toByteArray
import com.example.progressbt.ui.theme.ProgressBTTheme
import java.io.IOException
import java.io.OutputStream
import java.nio.charset.Charset
import java.util.UUID
import kotlinx.coroutines.*
import com.google.gson.Gson

class MainActivity : ComponentActivity() {

    private lateinit var bluetoothHandler: BluetoothHandler

    private val cmdLF: Byte = 10;

    private lateinit var textToSPrint: ByteArray;

    private val mainScope = MainScope() // Scope powiązany z głównym wątkiem
    private val apiResponse = mutableStateOf("Gotowy")
    private var isIntentHandled = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        bluetoothHandler = BluetoothHandler(this)
        bluetoothHandler.checkBluetoothPermissions()

        textToSPrint = stworzByteArrayZBajtem("textToSend", cmdLF)

        setContent {
            ProgressBTTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    Column(
                        modifier = Modifier
                            .fillMaxSize()
                            .padding(innerPadding)
                            .padding(16.dp)
                    ) {
                        Greeting(
                            name = apiResponse.value, // Wyświetlamy aktualną wartość stanu
                            modifier = Modifier.padding(bottom = 16.dp)
                        )
                        MyButton {
                            finish()
                        }
                        Button(onClick = { testPrint() }) {
                            Text("Test Print")
                        }
                    }
                }
            }
        }
        handleIntent(intent)
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        handleIntent(intent)
    }

    private fun handleIntent(intent: Intent?) {
        if (intent?.action == Intent.ACTION_VIEW && !isIntentHandled) {
            isIntentHandled = true
            val data = intent.data
            if (data != null) {
                val type = data.pathSegments.getOrNull(0)
                val id = data.pathSegments.getOrNull(1)
                if (type != null && id != null) {
                    apiResponse.value = "Łączenie z serwererm..."
                    val apiClient = ApiClient()
                    //val url = "http://mobile3.ifox.com.pl/api/print/$type/$id"
                    //val url = "https://192.168.33.2:7228/api/print/$type/$id"
                    val url = "http://192.168.33.2:5085/api/print/$type/$id"
                    apiClient.get(url) { response, exception ->
                        mainScope.launch(Dispatchers.Main) {
                            if (exception != null) {
                                apiResponse.value = "Error: ${exception.message}"
                            } else if (response != null) {
                                apiResponse.value = "Drukuję: ${response.info}"
                                val separator = cmdLF.toChar().toString()
                                var textToPrint = response.lines.joinToString(separator)

                                launch(Dispatchers.IO) {
                                    bluetoothHandler.print(textToPrint)
                                    withContext(Dispatchers.Main) {
                                        finish()
                                    }
                                }
                            } else {
                                apiResponse.value = "Unknown error"
                            }
                        }
                    }
                }
            }
        }
    }

    private fun testPrint() {
        mainScope.launch(Dispatchers.IO) {
            val textToPrint = "Test: łąć"
            val cp852Charset = Charset.forName("CP852")
            val dataToPrint = textToPrint.toByteArray(cp852Charset)

            bluetoothHandler.print(dataToPrint + cmdLF)
        }
    }

    @Composable
    fun Greeting(name: String, modifier: Modifier = Modifier) {
        Text(
            text = name, // Teraz 'name' jest dynamiczną wartością stanu
            modifier = modifier
        )
    }

    @Composable
    fun MyButton(onClick: () -> Unit) {
        Button(onClick = onClick) {
            Text("Close")
        }
    }

    @Preview(showBackground = true)
    @Composable
    fun GreetingPreview() {
        ProgressBTTheme {
            // Aby podgląd działał poprawnie ze stanem, możemy przekazać jakąś wartość
            Greeting(name = "Android (Podgląd)")
        }
    }

    @Preview(showBackground = true)
    @Composable
    fun MyButtonPreview() {
        ProgressBTTheme {
            MyButton {}
        }
    }

    fun stworzByteArrayZBajtem(tekst: String, bajtDoDodania: Byte): ByteArray {
        val tekstBytes = tekst.toByteArray(Charset.forName("UTF-8")) // Użyj odpowiedniego kodowania
        val wynik = ByteArray(tekstBytes.size + 1)
        System.arraycopy(tekstBytes, 0, wynik, 0, tekstBytes.size)
        wynik[tekstBytes.size] = bajtDoDodania
        return wynik
    }




}