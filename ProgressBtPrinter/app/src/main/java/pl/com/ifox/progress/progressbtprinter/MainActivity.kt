package pl.com.ifox.progress.progressbtprinter

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import kotlinx.coroutines.*
import pl.com.ifox.progress.progressbtprinter.ui.theme.ProgressBtPrinterTheme
import java.nio.charset.Charset

class MainActivity : ComponentActivity() {

    private lateinit var bluetoothHandler: BluetoothHandler

    private val cmdLF: Byte = 10;

    private val apiResponse = mutableStateOf("Gotowy")

    private var isIntentHandled = false

    private val mainScope = MainScope() // Scope powiązany z głównym wątkiem

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()

        bluetoothHandler = BluetoothHandler(this)
        bluetoothHandler.checkBluetoothPermissions()

        setContent {
            ProgressBtPrinterTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    Greeting(
                        name = apiResponse.value, // Wyświetlamy aktualną wartość stanu
                        modifier = Modifier.padding(innerPadding)
                    )
                    MyButton {
                        finish()
                    }
                    //Button(onClick = { testPrint() }) {
                        //Text("Test Print")
                    //}
                }
            }
        }

        handleIntent(intent);
    }


    @Composable
    fun Greeting(name: String, modifier: Modifier = Modifier) {
        Text(
            text = "Hello $name!",
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
        ProgressBtPrinterTheme {
            Greeting("Android")
        }
    }

    @Preview(showBackground = false)
    @Composable
    fun MyButtonPreview() {
        ProgressBtPrinterTheme {
            MyButton {}
        }
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
                    val url = "https://api.progress.ifox.com.pl/api/print/getPrintout/$id"
                     //val url = "https://192.168.33.2:7228/api/print/$type/$id"
                     //val url = "http://192.168.33.2:5085/api/print/$type/$id"
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
}