using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
   public float forwardSpeed = 3f;
   public float lateralSpeed = 3f;
   public float rotationSpeed = 10f;

   public Transform targetLocation;

   private bool obstacleDetected = false;

   private void Update()
   {
        if (!obstacleDetected)
        {
            MoveForward();
        }

        AvoidObstacle();
   }

   private void MoveForward()
   {
        Vector3 targetDirection = (targetLocation.position - transform.position).normalized;
        targetDirection.y = 0;

        transform.Translate(targetDirection * forwardSpeed * Time.deltaTime,  Space.World);

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
   }

   private void AvoidObstacle()
   {
        //Raycast Values
        float rayLength = 1f;
        float halfAngle = 75f;
        int numRays = 3;

        //Calculate the angle for the 3 raycasts
        float angleIncrement = 2f * halfAngle / (numRays - 1);

        //Trianglulize raycasts
        for (int  i = 0; i < numRays; i++)
        {
            float angle = -halfAngle + i + angleIncrement;
            
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;

            //Reform Raycast to current Direction
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, rayLength))
            {
                //Calculate avoidance direction

                Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal);
                avoidDirection.y = 0.0f;

                //Visualize rays
                Debug.DrawLine(transform.position, hit.point, Color.red);
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);

                obstacleDetected = true;
                break;
            }
        }

        if (obstacleDetected)
        {
            //Move Sideways only if there's no obstacle in the forward direction
            if(!Physics.Raycast(transform.position, transform.forward, rayLength))
            {
                MoveForward();
            }
            else
            {
                //Move left or right based on the obstacle position relative ot the AI
                if(Vector3.Dot(Vector3.Cross(Vector3.up, transform.forward), transform.right) < 0)
                {
                    MoveSideways(-transform.right); //Moving left
                }
                else
                {
                    MoveSideways(transform.right); //Moving right
                }
            }
        }
        else
        {
            obstacleDetected = false;
        }
   }

   private void MoveSideways(Vector3 direction)
   {
        transform.Translate(direction * lateralSpeed * Time.deltaTime, Space.World);
   }

}
