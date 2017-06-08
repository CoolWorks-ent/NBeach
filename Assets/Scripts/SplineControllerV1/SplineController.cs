using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eOrientationMode { NODE = 0, TANGENT }
public enum SplineState {Stopped, Loop, Reset, Paused, Resume, Active }

[AddComponentMenu("Splines/Spline Controller")]
[RequireComponent(typeof(SplineInterpolator))]
public class SplineController : MonoBehaviour
{
	public GameObject SplineRoot;
    public static SplineController instance = null;
    public float Duration = 10;
    public float Speed = 5;
	public eOrientationMode OrientationMode = eOrientationMode.NODE;
	public eWrapMode WrapMode = eWrapMode.ONCE;
	public bool AutoStart = true;
	public bool AutoClose = true;
	public bool HideOnExecute = true;
    public SplineState mSplineState;


	SplineInterpolator mSplineInterp;
	Transform[] mTransforms;

	void OnDrawGizmos()
	{
		Transform[] trans = GetTransforms();
		if (trans.Length < 2)
			return;
		SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
		SetupSplineInterpolator(interp, trans);
		interp.StartInterpolation(null, false, WrapMode);


		Vector3 prevPos = trans[0].position;
		for (int c = 1; c <= 100; c++)
		{
            //get position of 1st and last spline node
            //Duration = Speed * Time.deltaTime;
			float currTime = c * Duration / 100;
			Vector3 currPos = interp.GetHermiteAtTime(currTime);
			float mag = (currPos-prevPos).magnitude * 2;
			Gizmos.color = new Color(mag, 0, 0, 1);
			Gizmos.DrawLine(prevPos, currPos);
			prevPos = currPos;
		}
	}

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        mSplineInterp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
        switch(mSplineInterp.mSplineState)
        {
            case "Active":
                mSplineState = SplineState.Active;
                break;
            case "Loop":
                mSplineState = SplineState.Loop;
                break;
            case "Paused":
                mSplineState = SplineState.Paused;
                break;
            case "Stopped":
                mSplineState = SplineState.Stopped;
                break;
            case "Resume":
                mSplineState = SplineState.Resume;
                break;
            case "Reset":
                mSplineState = SplineState.Reset;
                break;
            default: break;
        }
       
        mTransforms = GetTransforms();

        if (HideOnExecute)
            DisableTransforms();

        if (AutoStart)
            FollowSpline();
        else
            sSplineState = SplineState.Paused;
    }

    
	void Start()
	{
		
    }

    public SplineState sSplineState
    {
        get { return mSplineState; }
        set {
            if (value == SplineState.Active)
            {
                mSplineState = SplineState.Active;
               // mSplineInterp.mSplineState = "Resume";
            }
            else if (value == SplineState.Loop)
            {
                mSplineState = SplineState.Loop;
                mSplineInterp.mSplineState = "Loop";
            }
            else if (value == SplineState.Paused)
            {
                mSplineState = SplineState.Paused;
                mSplineInterp.mSplineState = "Paused";
            }
            else if (value == SplineState.Reset)
            {
                mSplineState = SplineState.Reset;
                mSplineInterp.mSplineState = "Reset";
            }
            else if (value == SplineState.Resume)
            {
                mSplineState = SplineState.Resume;
                mSplineInterp.mSplineState = "Resume";
            }
            else if (value == SplineState.Stopped)
            {
                mSplineState = SplineState.Stopped;
                mSplineInterp.mSplineState = "Stopped";
            }
        }
    }

    void Update()
    {
       if(sSplineState == SplineState.Resume)
        {
            FollowSpline();
            sSplineState = SplineState.Active;
        }
    }

	void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		interp.Reset();
        //get distance between first and last nodes of spline
        float splineDistance = Vector3.Distance(interp.lastNode, interp.firstNode);
        Duration = splineDistance/Speed;
		float step = (AutoClose) ? Duration / trans.Length :
			Duration / (trans.Length - 1);

		int c;
		for (c = 0; c < trans.Length; c++)
		{
			if (OrientationMode == eOrientationMode.NODE)
			{
				interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
			}
			else if (OrientationMode == eOrientationMode.TANGENT)
			{
				Quaternion rot;
				if (c != trans.Length - 1)
					rot = Quaternion.LookRotation(trans[c + 1].position - trans[c].position, trans[c].up);
				else if (AutoClose)
					rot = Quaternion.LookRotation(trans[0].position - trans[c].position, trans[c].up);
				else
					rot = trans[c].rotation;

				interp.AddPoint(trans[c].position, rot, step * c, new Vector2(0, 1));
			}
		}

		if (AutoClose)
			interp.SetAutoCloseMode(step * c);
	}


	/// <summary>
	/// Returns children transforms, sorted by name.
	/// </summary>
	Transform[] GetTransforms()
	{
		if (SplineRoot != null)
		{
			List<Component> components = new List<Component>(SplineRoot.GetComponentsInChildren(typeof(Transform)));
			List<Transform> transforms = components.ConvertAll(c => (Transform)c);

			transforms.Remove(SplineRoot.transform);
			transforms.Sort(delegate(Transform a, Transform b)
			{
				return a.name.CompareTo(b.name);
			});

			return transforms.ToArray();
		}

		return null;
	}

	/// <summary>
	/// Disables the spline objects, we don't need them outside design-time.
	/// </summary>
	void DisableTransforms()
	{
		if (SplineRoot != null)
		{
			SplineRoot.SetActiveRecursively(false);
		}
	}


	/// <summary>
	/// Starts the interpolation
	/// </summary>
	void FollowSpline()
	{
		if (mTransforms.Length > 0)
		{
			SetupSplineInterpolator(mSplineInterp, mTransforms);
			mSplineInterp.StartInterpolation(null, true, WrapMode);
            mSplineInterp.mSplineState = "Resume";
		}
	}
}