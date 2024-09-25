namespace maxprofitness.shared
{
    public static class MaxProHandleState
    {
        /// <summary>
        /// This method is used to return the handle state of a MaxPro handle
        /// </summary>
        /// <param name="handleOldDistance"></param>
        /// <param name="handleNewDistance"></param>
        /// <returns></returns>
        public static HandleState GetHandleState(float handleOldDistance, float handleNewDistance)
        {
            const HandleState handleState = HandleState.None;

            switch (handleState)
            {
                case HandleState.None when handleOldDistance < handleNewDistance:
                {
                    return HandleState.Pulling;
                }
                case HandleState.None when handleOldDistance > handleNewDistance:
                {
                    return HandleState.Releasing;
                }
                default:
                {
                    return HandleState.Staying;
                }
            }
        }
    }
}

