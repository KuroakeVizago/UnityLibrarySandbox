using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Vizago.Camera
{

	public class CameraMovement : MonoBehaviour
	{


		[Space, Header("Config: ")] [Tooltip("What the camera will look at."), SerializeField]
		private Transform m_target;

		[Tooltip("How far the camera currently is from the m_target."), SerializeField]
		private float m_distance = 10.0f;

		[Tooltip("How fast the camera moves horizontally."), SerializeField]
		private float m_xSpeed = 1.0f;

		[Tooltip("How fast the camera moves vertically."), SerializeField]
		private float m_ySpeed = 1.0f;

		[Space, Header("Angle Limit:")] [Tooltip("Minimum angle of the camera on the _cameraY axis."), SerializeField]
		private float m_yMinLimit = 10f;

		[Tooltip("Maximum angle of the camera on the _cameraY axis."), SerializeField]
		private float m_yMaxLimit = 80f;

		[Tooltip("Minimum angle of the camera on the _cameraX axis."), SerializeField]
		private float m_xMinLimit = -360f;

		[Tooltip("Maximum angle of the camera on the _cameraX axis."), SerializeField]
		private float m_xMaxLimit = 360f;

		[Space, Header("Distance:")]
		[Tooltip("Minimum allowed m_distance between camera and m_target."), SerializeField]
		private float m_distanceMin = 0.5f;

		[Tooltip("Maximum allowed m_distance between camera and m_target"), SerializeField]
		private float m_distanceMax = 10f;

		[Space, Header("Collision:")]
		[Tooltip("Radius of the thin SphereCast, used to detect camera collisions."), SerializeField]
		private float m_thinRadius = 0.15f;

		[Tooltip("Radius of the thick SphereCast, used to detect camera collisions."), SerializeField]
		private float m_thickRadius = 0.3f;

		[Tooltip(
			 "LayerMask used for detecting camera collisions. Camera will not avoid objects if this is not set correctly."),
		 SerializeField]
		private LayerMask m_cameraCollisionLayer;

		private Quaternion _rotation;
		private Vector3 _position;
		private float _cameraX;
		private float _cameraY;

		void Start()
		{
			Vector3 angles = this.transform.eulerAngles;
			_cameraX = angles.y;
			_cameraY = angles.x;
		}

		void LateUpdate()
		{
			if (m_target)
			{
				CameraMove();
				_rotation = Quaternion.Euler(_cameraY, _cameraX, 0);

				if (m_distance < m_distanceMax)
				{
					m_distance = Mathf.Lerp(m_distance, m_distanceMax, Time.deltaTime * 2f);
				}

				Vector3 distanceVector = new Vector3(0.0f, 0.0f, -m_distance);
				Vector3 position = _rotation * distanceVector + m_target.position;
				transform.rotation = _rotation;
				transform.position = position;
				CameraCollision();
			}
		}

		public void CameraMove()
		{

			_cameraX += Mouse.current.delta.x.ReadValue() * m_xSpeed;
			_cameraY -= Mouse.current.delta.y.ReadValue() * m_ySpeed;

			_cameraX = ClampAngle(_cameraX, m_xMinLimit, m_xMaxLimit);
			_cameraY = ClampAngle(_cameraY, m_yMinLimit, m_yMaxLimit);
		}

		public float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
			{
				angle += 360F;
			}

			if (angle > 360F)
			{
				angle -= 360F;
			}

			return Mathf.Clamp(angle, min, max);
		}

		void CameraCollision()
		{
			Vector3 normal, thickNormal;
			Vector3 ray = transform.position - m_target.position;

			Vector3 collisionPoint = GetDoubleSphereCastCollision(transform.position, m_thinRadius, out normal, true);
			Vector3 collisionPointThick =
				GetDoubleSphereCastCollision(transform.position, m_thickRadius, out thickNormal, false);
			Vector3 collisionPointRay = GetRayCollisionPoint(transform.position);

			Vector3 collisionPointProjectedOnRay =
				Vector3.Project(collisionPointThick - m_target.position, ray.normalized) + m_target.position;
			Vector3 vectorToProject = (collisionPointProjectedOnRay - collisionPointThick).normalized;
			Vector3 collisionPointThickProjectedOnThin = collisionPointProjectedOnRay - vectorToProject * m_thinRadius;
			float thinToThickDistance = Vector3.Distance(collisionPointThickProjectedOnThin, collisionPointThick);
			float thinToThickDistanceNormal = thinToThickDistance / (m_thickRadius - m_thinRadius);

			float collisionDistanceThin = Vector3.Distance(m_target.position, collisionPoint);
			float collisionDistanceThick = Vector3.Distance(m_target.position, collisionPointProjectedOnRay);

			float collisionDistance =
				Mathf.Lerp(collisionDistanceThick, collisionDistanceThin, thinToThickDistanceNormal);

			// Thick point can be actually projected IN FRONT of the character due to double projection to avoid sphere moving through the walls
			// In this case we should only use thin point
			bool isThickPointIncorrect =
				transform.InverseTransformDirection(collisionPointThick - m_target.position).z > 0;
			isThickPointIncorrect = isThickPointIncorrect || (collisionDistanceThin < collisionDistanceThick);
			if (isThickPointIncorrect)
			{
				collisionDistance = collisionDistanceThin;
			}

			if (collisionDistance < m_distance)
			{
				m_distance = collisionDistance;
			}
			else
			{
				m_distance = Mathf.SmoothStep(m_distance, collisionDistance,
					Time.deltaTime * 100 * Mathf.Max(m_distance * 0.1f, 0.1f));
			}

			m_distance = Mathf.Clamp(m_distance, m_distanceMin, m_distanceMax);
			transform.position = m_target.position + ray.normalized * m_distance;

			if (Vector3.Distance(m_target.position, collisionPoint) >
			    Vector3.Distance(m_target.position, collisionPointRay))
			{
				transform.position = collisionPointRay;
			}
		}

		Vector3 GetDoubleSphereCastCollision(Vector3 cameraPosition, float radius, out Vector3 normal,
			bool pushAlongNormal)
		{
			float rayLength = 1;

			RaycastHit hit;
			Vector3 origin = m_target.position;
			Vector3 ray = origin - cameraPosition;
			float dot = Vector3.Dot(transform.forward, ray);

			if (dot < 0)
			{
				ray *= -1;
			}

			// Project the sphere in an opposite direction of the desired character->camera vector to get some space for the real spherecast
			if (Physics.SphereCast(origin, radius, ray.normalized, out hit, rayLength, m_cameraCollisionLayer))
			{
				origin = origin + ray.normalized * hit.distance;
			}
			else
			{
				origin += ray.normalized * rayLength;
			}

			// Do final spherecast with offset origin
			ray = origin - cameraPosition;
			if (Physics.SphereCast(origin, radius, -ray.normalized, out hit, ray.magnitude, m_cameraCollisionLayer))
			{
				normal = hit.normal;

				if (pushAlongNormal)
					return hit.point + hit.normal * radius;

				return hit.point;
			}

			normal = Vector3.zero;
			return cameraPosition;
		}

		Vector3 GetRayCollisionPoint(Vector3 cameraPosition)
		{
			Vector3 origin = m_target.position;
			Vector3 ray = cameraPosition - origin;

			RaycastHit hit;
			if (Physics.Raycast(origin, ray.normalized, out hit, ray.magnitude, m_cameraCollisionLayer))
			{
				return hit.point + hit.normal * 0.15f;
			}

			return cameraPosition;
		}
	}
}