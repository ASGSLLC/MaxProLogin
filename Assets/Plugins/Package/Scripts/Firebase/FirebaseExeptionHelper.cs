
using System;
#if FIREBASE
using Firebase;
#endif

namespace maxprofitness.login
{
    public class FirebaseExeptionHelper
    {
#if FIREBASE
        public static string HandleErrorExeption(Exception ex)
        {
            FirebaseException firebaseEx = ex.GetBaseException() as FirebaseException;
            string error = "";

            if (firebaseEx != null)
                error = firebaseEx.Message.ToString();

            if (string.IsNullOrEmpty(error))
                error = ex.GetBaseException().Message.ToString();

            return error;
#endif
        }
    }
}