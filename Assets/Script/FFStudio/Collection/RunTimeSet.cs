/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FFStudio
{
    public abstract class RuntimeSet< TKey, TValue > : ScriptableObject
    {
		[ SerializeField ] int size_list;
		[ SerializeField ] int size_dictionary;

		[ ShowInInspector, ReadOnly ] List< TValue > itemList;
		[ ShowInInspector, ReadOnly ] Dictionary< TKey, TValue > itemDictionary;

		public int ListCount            => itemList.Count;
		public int DictionaryValueCount => itemDictionary.Values.Count;
		public int DictionaryKeyCount   => itemDictionary.Values.Count;

		public void InitSet()
		{
			itemList       = new List< TValue >( size_list );
			itemDictionary = new Dictionary< TKey, TValue >( size_dictionary );
		}

		public void AddList( TValue value )
		{
#if UNITY_EDITOR
			if( !itemList.Contains( value ) )
				itemList.Add( value );
			else
				FFLogger.Log( "Trying to add same value to RunTime-LIST", value as Object );
#else
			itemList.Add( value );
#endif
		}

		public void RemoveList( TValue value )
		{
			itemList.Remove( value );
		}

		public void AddDictionary( TKey key, TValue value )
		{
#if UNITY_EDITOR
			if( !itemDictionary.ContainsKey( key ) )
				itemDictionary.Add( key, value );
			else
				FFLogger.Log( "Trying to add same value to RunTime-DICTIONARY", value as Object );
#else
			itemDictionary.Add( key, value );
#endif
		}
        
		public void RemoveDictionary( TKey key )
		{
			itemDictionary.Remove( key );
		}

		public void AddToBoth( TKey key, TValue value )
		{
			AddDictionary( key, value );
			AddList( value );
		}

		public void RemoveFromBoth( TKey key, TValue value )
		{
			RemoveDictionary( key );
			RemoveList( value );
		}

		[ Button ]
		public void ClearSet()
		{
			itemList.Clear();
			itemDictionary.Clear();
		}

		public TValue GetFromList( int index )
		{
			return itemList[ index ];
		}

		public TValue GetFromDictionary( TKey key )
		{
			TValue value;
			itemDictionary.TryGetValue( key, out value );

			return value;
		}
        
#if UNITY_EDITOR
		[ Button ]
		public void LogList()
		{
			foreach( var item in itemList )
				Debug.Log( item.ToString() );
		}

		[ Button ]
		public void LogDictionary()
		{
			foreach( var item in itemDictionary.Values )
				Debug.Log( item.ToString() );
		}
    }
#endif
}