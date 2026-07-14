using UnityEngine;
using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public static FirebaseFirestore Db { get; private set; }
    public static bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            Db = FirebaseFirestore.DefaultInstance;
            IsReady = true;
            Debug.Log("Firebase inicializado com sucesso!");
        }
        else
        {
            Debug.LogError("Firebase não pôde ser inicializado: " + dependencyStatus);
        }
    }
}