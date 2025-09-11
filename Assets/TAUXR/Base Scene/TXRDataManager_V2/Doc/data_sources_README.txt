# CONTINUOUSDATA_V2 - column <- source

# - Timing (Unity clock kept for continuity) -
timeSinceStartup                          <- Unity: Time.realTimeSinceStartup

# - Legacy head pose (Euler) from Head node -
Head_Position_x                            <- OVRPlugin.GetNodePoseState(step, Node.Head).Posef.Position.x
Head_Height                                <- ...Position.y
Head_Position_Z                            <- ...Position.z
Gaze_Pitch                                 <- Node.Head Posef.Orientation <- Euler.x (degrees)
Gaze_Yaw                                   <- ...Euler.y
Gaze_Roll                                  <- ...Euler.z
HeadNodeOrientationValid                   <- OVRPlugin.GetNodeOrientationValid(Node.Head)           (0/1)
HeadNodePositionValid                      <- OVRPlugin.GetNodePositionValid(Node.Head)              (0/1)
HeadNodeOrientationTracked                 <- OVRPlugin.GetNodeOrientationTracked(Node.Head)         (0/1)
HeadNodePositionTracked                    <- OVRPlugin.GetNodePositionTracked(Node.Head)            (0/1)
HeadNodeTime                               <- PoseStatef.Time returned by GetNodePoseState

# - Legacy gaze/raycast (unchanged semantics) -
FocusedObject                              <- App raycast (TXREyeTracker)
EyeGazeHitPosition_X                       <- Raycast hit.x
EyeGazeHitPosition_Y                       <- Raycast hit.y
EyeGazeHitPosition_Z                       <- Raycast hit.z

# - Eye gazes (dedicated API) -
RightEye_Pitch                             <- OVRPlugin.GetEyeGazesState(step,...).Right.Pose <- Euler.x
RightEye_Yaw                               <- ...Right.Pose <- Euler.y
LeftEye_Pitch                              <- ...Left.Pose  <- Euler.x
LeftEye_Yaw                                <- ...Left.Pose  <- Euler.y
LeftEye_IsValid                            <- EyeGazesState.Left.IsValid                              (0/1)
LeftEye_Confidence                         <- EyeGazesState.Left.Confidence                           (0..1)
LeftEye_Time                               <- EyeGazesState.Time                                      (seconds)
RightEye_IsValid                           <- EyeGazesState.Right.IsValid                             (0/1)
RightEye_Confidence                        <- EyeGazesState.Right.Confidence                          (0..1)
RightEye_Time                              <- EyeGazesState.Time

# - Recenter flags (system) -
shouldRecenter                             <- OVRPlugin.shouldRecenter                                (0/1)
recenterEvent                              <- Derived: rising edge of shouldRecenter                  (0/1 this frame)

# - Device nodes (for each <Node> in order: EyeLeft, EyeRight, EyeCenter, Head, HandLeft, HandRight, ControllerLeft, ControllerRight) -
Node_<Node>_Present                        <- OVRPlugin.GetNodePresent(<Node>)                        (0/1)
Node_<Node>_px / _py / _pz                 <- GetNodePoseState(step,<Node>).Posef.Position.{x,y,z}
Node_<Node>_qx / _qy / _qz / _qw           <- ...Posef.Orientation.{x,y,z,w}
Node_<Node>_Vel_x / _Vel_y / _Vel_z        <- OVRPlugin.GetNodeVelocity(<Node>).{x,y,z}
Node_<Node>_AngVel_x/_AngVel_y/_AngVel_z   <- OVRPlugin.GetNodeAngularVelocity(<Node>).{x,y,z}
Node_<Node>_Valid_Position                 <- OVRPlugin.GetNodePositionValid(<Node>)                  (0/1)
Node_<Node>_Valid_Orientation              <- OVRPlugin.GetNodeOrientationValid(<Node>)               (0/1)
Node_<Node>_Tracked_Position               <- OVRPlugin.GetNodePositionTracked(<Node>)                (0/1)
Node_<Node>_Tracked_Orientation            <- OVRPlugin.GetNodeOrientationTracked(<Node>)             (0/1)
Node_<Node>_Time                           <- PoseStatef.Time from GetNodePoseState

# - Hands (dedicated API; LEFT then RIGHT) -
LeftHand_Status                            <- OVRPlugin.GetHandState(step, Hand.HandLeft).Status
LeftHand_Root_px/_py/_pz                   <- HandState.RootPose.Position.{x,y,z}
LeftHand_Root_qx/_qy/_qz/_qw               <- HandState.RootPose.Orientation.{x,y,z,w}
LeftHand_HandScale                         <- HandState.HandScale
LeftHand_HandConfidence                    <- HandState.HandConfidence
LeftHand_FingerConf_Thumb                  <- HandState.FingerConfidences[Thumb]
LeftHand_FingerConf_Index                  <- HandState.FingerConfidences[Index]
LeftHand_FingerConf_Middle                 <- HandState.FingerConfidences[Middle]
LeftHand_FingerConf_Ring                   <- HandState.FingerConfidences[Ring]
LeftHand_FingerConf_Pinky                  <- HandState.FingerConfidences[Pinky]
LeftHand_RequestedTS                       <- HandState.RequestedTimeStamp
LeftHand_SampleTS                          <- HandState.SampleTimeStamp

# Per-bone (arrays; index based, order = SDK bone enum order)
LeftHand_BonePos_[ii]_x/_y/_z              <- HandState.Vector3f[] BonePositions[ii].{x,y,z}
LeftHand_BoneRot_[ii]_qx/_qy/_qz/_qw       <- HandState.Quatf[]   BoneRotations[ii].{x,y,z,w}

RightHand_*                                <- Same fields as LeftHand (Hand.HandRight)

# - Body (BodyState4; frame + joints) -
Body_Time                                   <- OVRPlugin.GetBodyState4(step,...).Time
Body_Confidence                             <- state.Confidence
Body_Fidelity                               <- state.Fidelity
Body_CalibrationStatus                      <- state.CalibrationStatus
Body_SkeletonChangedCount                   <- state.SkeletonChangedCount

# For each joint J in SDK enum order:
Body_Joint_<J>_px/_py/_pz                       <- state.JointLocations[J].Posef.Position.{x,y,z}
Body_Joint_<J>_qx/_qy/_qz/_qw                   <- state.JointLocations[J].Posef.Orientation.{x,y,z,w}
Body_Joint_<J>_Flags                            <- state.JointLocations[J].LocationFlags  (bitfield)

# - Perf -
AppMotionToPhotonLatency                    <- OVRPlugin.AppMotionToPhotonLatency (ms, if available)

# - Custom transforms (experiment-specific; registered order) -
Custom_<Name>_px/_py/_pz                    <- Unity Transform.position.{x,y,z}
Custom_<Name>_qx/_qy/_qz/_qw                <- Unity Transform.rotation.{x,y,z,w}



# FACEEXPRESSIONS_V2 - column <- source

# - Timing -
timeSinceStartup                          <- Unity: Time.realTimeSinceStartup (renamed from "TimeFromStart") [legacy continuity]

# - Face state (dedicated API: FaceState2) -
Face_Time                                 <- FaceState2.Time         (sdk timestamp, seconds)
Face_Status                               <- FaceState2.Status       (flags bitfield)

# - Expression weights (OVRPlugin.FaceExpression2 names, one per entry) -
Brow_Lowerer_L ... Tongue_Retreat           <- FaceState2.ExpressionWeights[i] (int 0..69, one per enum entry)

# - Region confidences (OVRPlugin.FaceRegionConfidence) -
FaceRegionConfidence_Upper, FaceRegionConfidence_Lower <- FaceState2.ExpressionWeightConfidences[Upper/Lower] (float 0..1)


# Notes:
# - Indices map to the FaceExpression enum order in the MetaXR v77 SDK guide.
# - Total per frame = 70 weights + 2 confidences + status + time + timeSinceStartup.