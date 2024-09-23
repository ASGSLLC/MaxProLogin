using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace maxprofitness.shared
{
    public class FirestoreDatabaseManager : Singleton<FirestoreDatabaseManager>
    {
        #region VARIABLES


        public static FirebaseFirestore db;
        public static bool isOnline = false;

        private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);


        #endregion


        #region MONBEHAVIOURS


        //----------------------------------//
        protected override void Awake()
        //----------------------------------//
        {
            base.Awake();

        } // END Awake


        #endregion


        #region CHECK DEPENDENCIES


        //----------------------------------//
        public Coroutine CheckDependecies(Action OnSucess = null)
        //----------------------------------//
        {
            return StartCoroutine(ICheckDependecies(OnSucess));

        } // END CheckDependecies


        //----------------------------------//
        private IEnumerator ICheckDependecies(Action OnSucess = null)
        //----------------------------------//
        {
            //Debug.Log("FirestoreDatabaseManager//ICheckDependecies//");

            if (isOnline == false)
            {
                var task = FirebaseFirestore.DefaultInstance.EnableNetworkAsync();

                while (task.IsCompleted == false) { yield return waitForSeconds; }

                isOnline = true;
            }

            //Debug.Log("FirestoreDatabaseManager//ICheckDependecies// network enabled");

            if (db == null)
            {
                FirebaseDependencyHelper.CheckAndFixDependenciesAsync(() =>
                {
                    //Debug.Log("FirestoreDatabaseManager//ICheckDependecies// got db");

                    db = FirebaseFirestore.DefaultInstance;
                });
            }
            else
            {
                //Debug.Log("FirestoreDatabaseManager//ICheckDependecies// have db");
            }

            while (db == null) { yield return waitForSeconds; }

            OnSucess?.Invoke();

        } // END CheckDependecies


        #endregion


        #region SAVE DOCUMENT


        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="document"></param>
        /// <param name="data"></param>
        ///  data ex Dictionary<string, object> city = new Dictionary<string, object>
        ///  {
        ///     { "Name", "Los Angeles" },
        ///     { "State", "CA" },
        ///     { "Country", "USA" }
        ///  };
        //----------------------------------//
        public Coroutine SaveDocument(DocumentReference docRef, Dictionary<string, object> data, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            return StartCoroutine(ISaveDocument(docRef, data, OnSuccess, OnError));

        } // END SaveData


        //----------------------------------//
        private IEnumerator ISaveDocument(DocumentReference docRef, Dictionary<string, object> data, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            Debug.Log("FirestoreDatabaseManager//ISaveDocument//");

            yield return StartCoroutine(ICheckDependecies());

            var task = docRef.SetAsync(data);

            yield return new WaitUntil(() => task.IsCompleted);

            Debug.Log("FirestoreDatabaseManager//ISaveDocument// task complete");

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("FirestoreDatabaseManager//ISaveDocument// error: " + error);
            }
            else
            {
                OnSuccess?.Invoke();
                Debug.Log("FirestoreDatabaseManager//ISaveDocument// Data Saved");
            }

        } // END ISaveData


        #endregion


        #region GET DOCUMENT


        //----------------------------------//
        public Coroutine GetDocument(DocumentReference docRef, Action<DocumentSnapshot> OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            return StartCoroutine(IGetDocument(docRef, OnSuccess, OnError));

        } // END GetData


        //----------------------------------//
        private IEnumerator IGetDocument(DocumentReference docRef, Action<DocumentSnapshot> OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            yield return StartCoroutine(ICheckDependecies());

            var task = docRef.GetSnapshotAsync();

            yield return new WaitUntil(() => task.IsCompleted);

            Debug.Log("FirestoreDatabaseManager//IGetDocument// task complete");

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("FirestoreDatabaseManager//IGetDocument// error: " + error);
            }
            else
            {
                Debug.Log("FirestoreDatabaseManager//IGetDocument// got data " + task.Result.Id);

                OnSuccess?.Invoke(task.Result);
            }

        } // END IGetData 


        #endregion


        #region GET COLLECTION


        //----------------------------------//
        public Coroutine GetCollection(CollectionReference userRef, Action<IEnumerable<DocumentSnapshot>> OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            return StartCoroutine(IGetCollection(userRef, OnSuccess, OnError));

        } // END GetCollection

        //----------------------------------//
        private IEnumerator IGetCollection(CollectionReference usersRef, Action<IEnumerable<DocumentSnapshot>> OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            yield return StartCoroutine(ICheckDependecies());

            var task = usersRef.GetSnapshotAsync();

            yield return new WaitUntil(() => task.IsCompleted);

            //Debug.Log("FirestoreDatabaseManager//IGetCollection// task complete");

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("FirestoreDatabaseManager//IGetCollection// error: " + error);
            }
            else
            {
                //  Debug.Log("FirestoreDatabaseManager//IGetCollection// go data at path " + usersRef.Id);

                QuerySnapshot snapshot = task.Result;

                //Debug.Log("FirestoreDatabaseManager//IGetCollection// Read all data from the users collection.");

                OnSuccess?.Invoke(snapshot.Documents);
            }

        } // END IGetCollection 


        #endregion


        #region UPDATE DOCUMENT


        //----------------------------------//
        public Coroutine UpdateDocument(DocumentReference docRef, string field, object value, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            return StartCoroutine(IUpdateDocument(docRef, field, value, OnSuccess, OnError));

        } // END UpdateDocument


        //----------------------------------//
        private IEnumerator IUpdateDocument(DocumentReference docRef, string field, object value, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            Debug.Log("FirestoreDatabaseManager//IUpdateDocument//");

            yield return StartCoroutine(ICheckDependecies());

            var task = docRef.UpdateAsync(field, value);

            yield return new WaitUntil(() => task.IsCompleted);

            Debug.Log("FirestoreDatabaseManager//IUpdateDocument// task complete");

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("FirestoreDatabaseManager//IUpdateDocument// error: " + error);
            }
            else
            {
                OnSuccess?.Invoke();
                Debug.Log("FirestoreDatabaseManager//IUpdateDocument// Data Saved");
            }

        } // END IUpdateDocument


        #endregion


        #region DELETE DOCUMENT


        //----------------------------------//
        public Coroutine DeleteDocument(DocumentReference docRef, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            return StartCoroutine(IDeleteDocument(docRef, OnSuccess, OnError));

        } // END DeleteDocument


        //----------------------------------//
        private IEnumerator IDeleteDocument(DocumentReference docRef, Action OnSuccess = null, Action<string> OnError = null)
        //----------------------------------//
        {
            yield return StartCoroutine(ICheckDependecies());

            var task = docRef.DeleteAsync();

            yield return new WaitUntil(() => task.IsCompleted);

            Debug.Log("FirestoreDatabaseManager//IDeleteData// task complete");

            if (task.Exception != null || task.IsCanceled)
            {
                string error = FirebaseExeptionHelper.HandleErrorExeption(task.Exception);

                OnError?.Invoke(error);
                Debug.LogError("FirestoreDatabaseManager//IDeleteData// error: " + error);
            }
            else
            {
                Debug.Log("FirestoreDatabaseManager//IDeleteData// data deleted ");

                OnSuccess?.Invoke();
            }

        } // END IDeleteDocument 


        #endregion


    } // END Class
}