using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;

public class OpenAIAPITest : MonoBehaviour
{
    public InputField inputField;
    
    // OpenAI API密鑰
    private string apiKey = "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";

    // OpenAI API端點
    private string apiEndpoint = "https://api.openai.com/v1/chat/completions";

    // 請求文本
    public string requestText = "Say this is a test";

    private OpenAIRequest openAIRequest = new OpenAIRequest();
    private MessageObj message = new MessageObj();

    public void OnClick() 
    {
        requestText = inputField.textComponent.text;

        // 設置請求參數
        openAIRequest.model = "gpt-3.5-turbo";
        MessageObj message = new MessageObj();
        message.role = "user";
        message.content = requestText;
        openAIRequest.messages.Add(message);
        openAIRequest.temperature = 1f;
        openAIRequest.max_tokens = 128;
        openAIRequest.n = 1;
        openAIRequest.user = "user";

        // 發送請求
        StartCoroutine(SendRequest(ParamsToRequest(openAIRequest)));
    }

    public UnityWebRequest ParamsToRequest(OpenAIRequest requestParams)
    {
        // 將請求參數序列化為JSON數據
        string json = JsonMapper.ToJson(requestParams);

        // 創建UnityWebRequest對象
        UnityWebRequest request = UnityWebRequest.PostWwwForm(apiEndpoint, json);

       // 設置請求頭
       request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 設置請求體
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        return request;
    }

    IEnumerator SendRequest(UnityWebRequest request)
    {
        // 發送請求
        yield return request.SendWebRequest();

        // 處理響應
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log($"Error: {request.error} content: {request.downloadHandler.text}");
        }
        else
        {
            // 將響應數據反序列化為OpenAIResponse對象
            string responseJson = request.downloadHandler.text;
            OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseJson);

            // 在控制台中打印響應結果
            Debug.Log("Response: " + response.choices[0].message.content);
        }
    }
}

[System.Serializable]
public class OpenAIRequest 
{
    public string model;
    public List<MessageObj> messages;
    public double temperature;
    public int max_tokens;
    public int n;
    public string user;
}

[System.Serializable]
public class MessageObj
{
    public string role;
    public string content;
}

[System.Serializable]
public class OpenAIResponse
{
    public string id;
    public int created;
    public string model;
    public UsageObj usage;
    public List<Choice> choices;

    [System.Serializable]
    public class Choice
    {
        public int index;
        public string finish_reason;
        public MessageObj message;
    }
    [System.Serializable]
    public class UsageObj
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}