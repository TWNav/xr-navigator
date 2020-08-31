using System.Collections.Generic;

public static class DictionaryExtensions {
	public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {	
		TValue obj;
		dict.TryGetValue(key, out obj);	
		return obj;
	}
}