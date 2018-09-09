﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace OrX.chase
{
	class DeadSpaceDetection : IDetection
	{
		public DeadSpaceDetection ()
		{
		}

		public IEnumerable<Collider> GetComponents(Vector3 center, float radius){
			
			Collider[] hitColliders = Physics.OverlapSphere(center, radius);
			int i = 0;
			while (i < hitColliders.Length) {
				yield return hitColliders [i];
			}
		}

		public void UpdateMap(List<OrXchaseContainer> containers){

			var str = "";

			foreach (var container in containers) {
				Rigidbody body = null;
				container.EVA.GetComponentCached<Rigidbody> (ref body);

				foreach (var collision in GetComponents(body.position, 1)) {
					str += collision.gameObject.name + Environment.NewLine;
				}
			}


			OrXchaseController.instance.debug = str;
		}

		public void Debug(){
			
		}
	}
}

