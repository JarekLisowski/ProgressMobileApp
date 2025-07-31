package com.example.progressbt

import com.google.gson.Gson
import okhttp3.Call
import okhttp3.Callback
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import okhttp3.Response
import java.io.IOException

data class ApiResponse(
    val info: String,
    val lines: List<String>
)

class ApiClient {

    private val client = OkHttpClient()
    private val gson = Gson()

    fun post(url: String, json: String, callback: (ApiResponse?, Exception?) -> Unit) {
        val request = Request.Builder()
            .url(url)
            .post(json.toRequestBody("application/json; charset=utf-8".toMediaType()))
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                callback(null, e)
            }

            override fun onResponse(call: Call, response: Response) {
                if (!response.isSuccessful) {
                    callback(null, IOException("Unexpected code ${response}"))
                    return
                }

                val body = response.body?.string()
                if (body == null) {
                    callback(null, IOException("Empty response body"))
                    return
                }

                try {
                    val apiResponse = gson.fromJson(body, ApiResponse::class.java)
                    callback(apiResponse, null)
                } catch (e: Exception) {
                    callback(null, e)
                }
            }
        })
    }

    fun get(url: String, callback: (ApiResponse?, Exception?) -> Unit) {
        val request = Request.Builder()
            .url(url)
            .build()

        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                callback(null, e)
            }

            override fun onResponse(call: Call, response: Response) {
                if (!response.isSuccessful) {
                    callback(null, IOException("Unexpected code ${response}"))
                    return
                }

                val body = response.body?.string()
                if (body == null) {
                    callback(null, IOException("Empty response body"))
                    return
                }

                try {
                    val apiResponse = gson.fromJson(body, ApiResponse::class.java)
                    callback(apiResponse, null)
                } catch (e: Exception) {
                    callback(null, e)
                }
            }
        })
    }
}
