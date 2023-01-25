namespace shunda.Core.Util;

public class PassUtil {
    /// <summary>
    /// 是否有对某资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResIdMap"></param>
    /// <param name="isAdmin"></param>
    /// <param name="passResId"></param>
    /// <returns></returns>
    public static bool HasPassRes(IDictionary<int, bool> passResIdMap, bool isAdmin, int passResId) {
        if (passResIdMap == null) {
            return false;
        }

        if (isAdmin) {
            return true;
        }

        return passResIdMap.ContainsKey(passResId);
    }

    /// <summary>
    /// 是否有对其中某个资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResIdMap"></param>
    /// <param name="isAdmin"></param>
    /// <param name="passResIds"></param>
    /// <returns></returns>
    public static bool HasOneOfPassRes(IDictionary<int, bool> passResIdMap, bool isAdmin, params int[] passResIds) {
        if (passResIdMap == null) {
            return false;
        }

        if (isAdmin) {
            return true;
        }

        foreach (int passResId in passResIds) {
            if (passResIdMap.TryGetValue(passResId, out bool grant) && grant) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 是否有对其中某个资源的授权
    /// 应预先对 PassResIdMap 设值
    /// </summary>
    /// <param name="passResIdMap"></param>
    /// <param name="isAdmin"></param>
    /// <param name="passResIdList"></param>
    /// <returns></returns>
    public static bool HasOneOfPassRes(IDictionary<int, bool> passResIdMap, bool isAdmin, IList<int> passResIdList) {
        return HasOneOfPassRes(passResIdMap, isAdmin, passResIdList.ToArray());
    }
}