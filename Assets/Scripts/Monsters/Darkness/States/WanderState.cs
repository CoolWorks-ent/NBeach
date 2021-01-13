using UnityEngine;
using System.Collections;

namespace DarknessMinion
{


	[CreateAssetMenu(menuName = "AI/Darkness/State/WanderState")]
	public class WanderState : ChaseState
	{
		//[Range(8,20)]
		//public int relativeRangeToPlayer;
		[Range(1, 8)]
		public int wanderRadius;
		[Range(0, 10)]
		public float timeToPickNextPoint;

		public override void InitializeState(Darkness darkController)
		{
			base.InitializeState(darkController);
			//AI_Manager.OnRequestNewTarget(controller.creationID);
			//darkController.navTarget.ReleaseTarget();
			//PopulatePatrolPoints(5, darkController);
			//darkController.animeController.SetTrigger(darkController.chaseHash);
			//controller.navTarget = ChooseNewPatrolPoint(wanderRadius);
			//darkController.AddCooldown(new CooldownInfo(timeToPickNextPoint, CooldownStatus.Patrolling, CooldownCallback));
			//darkController.pather.destination = darkController.navTarget.GetPosition();
			//darkController.pather.canMove = true;
			//darkController.pather.canSearch = true;
			//darkController.pather.pickNextWaypointDist = 0.95f;
			//controller.pather.repathRate = timeToPickNextPoint;
			//controller.aIMovement.wandering = true;
			//ChooseNewPatrolPoints(controller);
			/*controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
			AI_Manager.Instance.StartCoroutine(NewPointBuffer(controller,5));
			controller.aIMovement.CreatePath(controller.Target.position);
			controller.aIMovement.moving = true;
			controller.aIMovement.speed = 10;*/
		}

		public override void UpdateState(Darkness controller)
		{
			//Check if the Darkness has finished moving then assign it a new location
			// if(controller.aIMovement.aI.remainingDistance <= 3)
			// {
			//     ChooseNewPatrolPoints(controller);
			//     controller.aIMovement.wayPoint = ChoosePatrolPoint(controller);
			// }
			//controller.aIMovement.UpdatePath(controller.Target.position);
			/*if (controller.pather.reachedEndOfPath || controller.navTargetDist <= controller.attackDist)
			{
				//AI_Manager.OnRequestNewTarget(controller.creationID);
				controller.pather.destination = controller.navTarget.GetPosition();
			}
			if (controller.playerDist < 3 && controller.pather.rotationSpeed < 360)
				controller.pather.rotationSpeed = 400;
			CheckTransitions(controller);*/
		}

		public override void MovementUpdate(Darkness controller)
		{

		}

		protected override void CooldownCallback(Darkness controller)
		{
			//controller.navTarget = ChooseNewPatrolPoint(wanderRadius);
		}

		/*private NavigationTarget ChooseNewPatrolPoint(float radius)
		{
			NavigationTarget navTarget = new NavigationTarget(Vector3.zero, DarknessManager.Instance.oceanPlane.position.y, NavigationTarget.NavTargetTag.Patrol);
			Vector3 t = Random.insideUnitSphere * wanderRadius; //subtract the player position from Darkness position. Add some offset to get a starting point a bit ahead of the player

			navTarget.UpdateLocation(t);

			return navTarget;
		}*/

		/*public void PopulatePatrolPoints(int size, Darkness controller)
		{
			controller.patrolPoints = new NavigationTarget[size];
			for (int i = 0; i < controller.patrolPoints.Length; i++)
			{
				float xOffset = 0;
				//PatrolPoints[i].position.parent = this.transform;
				if (i % 2 == 0 || i == 0)
					xOffset = controller.transform.position.x - UnityEngine.Random.Range(5 + i, 15);
				else xOffset = controller.transform.position.x + UnityEngine.Random.Range(5 + i, 15);
				Vector3 offset = new Vector3(xOffset, controller.transform.position.y, controller.transform.position.z - UnityEngine.Random.Range(9, 9 + i));
				controller.patrolPoints[i] = new NavigationTarget(controller.transform.position, offset, Vector3.zero, DarknessManager.Instance.oceanPlane.position.y, NavigationTarget.NavTargetTag.Patrol);
				//PatrolPoints[i].targetID = i;
			}
		}*/

		/*private void ChooseNewPatrolPoints(Darkness controller)
		{
			Vector3 direction =  (controller.Target.transform.position - controller.transform.position).normalized;
			Vector3 wTemp = controller.aIMovement.wayPoint + direction*relativeRangeToPlayer;
			Vector3[] temp = new Vector3[controller.aIMovement.PatrolPoints.Length];

			for(int i = 0; i < temp.Length; i++)
			{
				Vector3 t = Random.insideUnitSphere * wanderRadius;
				controller.aIMovement.PatrolPoints[i] = new Vector3(t.x, controller.transform.position.y, t.z);
			}
			controller.aIMovement.PatrolPoints = temp;
		}

		private Vector3 ChoosePatrolPoint(Darkness controller)
		{
		   return controller.aIMovement.PatrolPoints[Random.Range(0,controller.aIMovement.PatrolPoints.Length)];
		}
		*/

		public override void ExitState(Darkness darkController)
		{
			//controller.aIMovement.EndMovement();
			//controller.aIMovement.wandering = false;
			//darkController.pather.canMove = false;
			//darkController.pather.canSearch = false;
			//darkController.sekr.CancelCurrentPathRequest();
			//controller.animeController.ResetTrigger(controller.chaseHash);
			base.ExitState(darkController);
		}
	}
}