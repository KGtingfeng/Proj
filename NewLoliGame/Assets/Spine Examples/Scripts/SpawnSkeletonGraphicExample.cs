/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated January 1, 2020. Replaces all prior versions.
 *
 * Copyright (c) 2013-2020, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Examples {
	public class SpawnSkeletonGraphicExample : MonoBehaviour {

		public SkeletonDataAsset skeletonDataAsset;

		[SpineAnimation(dataField: "skeletonDataAsset")]
		public string startingAnimation;

		[SpineSkin(dataField: "skeletonDataAsset")]
		public string startingSkin = "base";
		public Material skeletonGraphicMaterial;

        //IEnumerator Start () {
        //	if (skeletonDataAsset == null) yield break;
        //	skeletonDataAsset.GetSkeletonData(false); // Preload SkeletonDataAsset.
        //	yield return new WaitForSeconds(1f); // Pretend stuff is happening.

        //	var sg = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAsset, this.transform, skeletonGraphicMaterial); // Spawn a new SkeletonGraphic GameObject.
        //	sg.gameObject.name = "SkeletonGraphic Instance";

        //	// Extra Stuff
        //	sg.Initialize(false);
        //	sg.Skeleton.SetSkin(startingSkin);
        //	sg.Skeleton.SetSlotsToSetupPose();
        //	sg.AnimationState.SetAnimation(0, startingAnimation, true);
        //}


        IEnumerator Start()
        {
            //string path = "Game/Spine Skeletons/spineboy-unity/Test";
            //Object tmpObj =  Resources.Load(path);
            //GameObject spineObj = Instantiate( tmpObj) as GameObject;
            //spineObj.layer = 5;
            //spineObj.SetActive(true);



            // Object skeleton= Resources.Load(path);

            string path = "Game/Spine Skeletons/spineboy-unity/spineboy-unity_SkeletonData";
            //Debug.Log("skeleton " + skeleton);
            SkeletonDataAsset skeletonDataAssets = Resources.Load<SkeletonDataAsset>(path);
            Debug.Log("load by Type: " + skeletonDataAssets);

            Material material = Resources.Load<Material>("Game/Spine Skeletons/spineboy-unity/spineboy_Material");
            Debug.Log("material " + material);
            if (skeletonDataAssets != null)
                skeletonDataAssets.GetSkeletonData(false);
            yield return new WaitForSeconds(1f);


            var sg = SkeletonGraphic.NewSkeletonGraphicGameObject(skeletonDataAssets, transform, material); // Spawn a new SkeletonGraphic GameObject.
            sg.gameObject.name = "SkeletonGraphic Instance";

            // Extra Stuff
            sg.Initialize(false);
            sg.Skeleton.SetSlotsToSetupPose();
            sg.AnimationState.SetAnimation(0, "idle", true);
            sg.Skeleton.SetSkin("base");
            sg.gameObject.layer = 5;



        }
    }

}
