#if FIREBASE_STORAGE
using Firebase;
using Firebase.Storage;
using System;
using System.Collections;
using UnityEngine;
using maxprofitness.login;
public class CloudStorageManager : Singleton<CloudStorageManager>
{
    #region VARIABLES
    public static Action OnCheckAndFixDependeciesAvailable;
    public static Action<string> OnCheckAndFixDependeciesUnavailable;

    public long maxAllowedDownloadSize = 2000000;

    private StorageReference storageRef;
    private FirebaseStorage storage;

    public DependencyStatus dependencyStatus;
    #endregion


    //----------------------------------//
    public Coroutine UploadFromMemmory(byte[] bytes, string uploadPath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("CloudStorageManager//UploadFromMemmory");

        return StartCoroutine(IUploadFromMemmory(bytes, uploadPath, OnSuccess, OnError));

    } // END UploadFromMemmory

    //----------------------------------//
    private IEnumerator IUploadFromMemmory(byte[] bytes, string uploadPath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        yield return StartCoroutine(CheckDependecies());

        StorageReference uploadRef = storageRef.Child(uploadPath);
        var task = uploadRef.PutBytesAsync(bytes);
       
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("CloudStorageManager//UploadFromMemmory// " + error);
            OnError?.Invoke(error);
        }
        else
        {   
            // Metadata contains file metadata such as size, content-type, and md5hash.
            StorageMetadata metadata = task.Result;
            string md5Hash = metadata.Md5Hash;
            Debug.Log("CloudStorageManager//UploadFromMemmory//Finished uploading...");
            Debug.Log("CloudStorageManager//UploadFromMemmory//md5 hash = " + md5Hash);
            OnSuccess?.Invoke();
        }

    } // END IUploadFromMemmory

    //----------------------------------//
    public Coroutine DownloadToMemmory(string storagePath, Action<byte[]> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("CloudStorageManager//DownloadToMemmory");

        return StartCoroutine(IDownloadToMemmory(storagePath, OnSuccess, OnError));

    } // END DownloadToMemmory

    //----------------------------------//
    private IEnumerator IDownloadToMemmory(string storagePath, Action<byte[]> OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        byte[] data = null;

        yield return StartCoroutine(CheckDependecies());

        StorageReference downloadRef = storageRef.Child(storagePath);

        Debug.Log("CloudStorageManager//IDownloadToMemmory// downloadRef " + downloadRef.Path);
        Debug.Log("CloudStorageManager//IDownloadToMemmory// storageRef " + storageRef.Path);

        var task = downloadRef.GetBytesAsync(maxAllowedDownloadSize);

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("CloudStorageManager//DownloadToMemmory// error: " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("CloudStorageManager//DownloadToMemmory// complete ");
            data = task.Result; 
            OnSuccess?.Invoke(data);
        }

    } // END IDownloadToMemmory

    //----------------------------------//
    public Coroutine DownloadToCahce(string storagePath, string cachePath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        Debug.Log("CloudStorageManager//DownloadToMemmory");

        return StartCoroutine(IDownloadToCache(storagePath, cachePath, OnSuccess, OnError));

    } // END DownloadToCahce

    //----------------------------------//
    private IEnumerator IDownloadToCache(string storagePath, string cachePath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        yield return StartCoroutine(CheckDependecies());

        StorageReference downloadRef = storageRef.Child(storagePath);

        var task = downloadRef.GetFileAsync(cachePath);

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("CloudStorageManager//DownloadToMemmory// error: " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("CloudStorageManager//DownloadToMemmory// complete ");
            OnSuccess?.Invoke();
        }

    } // END IDownloadToCache

    //----------------------------------//
    private IEnumerator CheckDependecies()
    //----------------------------------//
    {
        if (storageRef == null)
            FirebaseDependencyHelper.CheckAndFixDependenciesAsync(() =>
            {
                storage = FirebaseStorage.DefaultInstance;
                storageRef = storage.RootReference;
            });

        while (storageRef == null) { yield return null; }

    } // END CheckDependecies

    //----------------------------------//
    public Coroutine DeleteObject(string storagePath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IDeleteObject(storagePath, OnSuccess, OnError));

    } // END IDownloadToMemmory

    //----------------------------------//
    private IEnumerator IDeleteObject(string storagePath, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        yield return StartCoroutine(CheckDependecies());

        StorageReference deleteRef = storageRef.Child(storagePath);
        
        Debug.Log("CloudStorageManager//IDeleteObject// deleteRef " + deleteRef.Path);
        Debug.Log("CloudStorageManager//IDeleteObject// storageRef " + storageRef.Path);

        var task = deleteRef.DeleteAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

            Debug.LogError("CloudStorageManager//IDeleteObject// error: " + error);

            OnError?.Invoke(error);
        }
        else
        {
            Debug.Log("CloudStorageManager//IDeleteObject// complete ");
            OnSuccess?.Invoke();
        }

    } // END IDownloadToMemmory

    //----------------------------------//
    public Coroutine DeleteObjects(string[] storagePaths, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        return StartCoroutine(IDeleteObjects(storagePaths, OnSuccess, OnError));

    } // END IDownloadToMemmory

    //----------------------------------//
    private IEnumerator IDeleteObjects(string[] storagePaths, Action OnSuccess = null, Action<string> OnError = null)
    //----------------------------------//
    {
        bool isError = false;

        foreach (string path in storagePaths) 
        {
            yield return DeleteObject(path,null,(error) => 
                {
                    isError = true;
                    OnError?.Invoke(error);
                });

            if(isError)
                yield break;
        }

        OnSuccess.Invoke();

    } // END IDownloadToMemmory

} // END Class
#endif