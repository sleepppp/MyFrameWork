using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JustAssets.TerrainUtility
{
    /// <summary>
    ///     Merges terrain in nxn pieces.
    /// </summary>
    public sealed class MergeTerrain : IValidate<Terrain>
    {
        private const int MAXIMUM_HEIGHTMAP_RESOLUTION = 4097;

        private const int MAXIMUM_CONTROL_TEXTURE_RESOLUTION = 4096;

        private const string WRONG_SELECTION =
            "You need to select at least 4 terrains. You need to select n^2 terrains (4, 16, 64, 256, ...). You selected {0} pieces.";

        private const int MAXIMUM_BASEMAP_TEXTURE_RESOLUTION = 4096;

        public Terrain[] Selection { get; set; }

        public bool IsValid(out string reason)
        {
            if (Selection == null)
            {
                reason = "Select some terrain.";
                return false;
            }

            var valid = IsValid();
            reason = valid ? null : string.Format(WRONG_SELECTION, Selection.Length);
            return valid;
        }

        public void Merge()
        {
            if (!IsValid())
            {
                EditorUtility.DisplayDialog("Error", string.Format(WRONG_SELECTION, Selection.Length), "Cancel");
                return;
            }

            var terrainsPerAxis = (int) Math.Sqrt(Selection.Length);
            var sorted = DetermineTerrainPositions(Selection, terrainsPerAxis);

            if (sorted.Cast<Terrain>().Any(terrain => terrain == null))
            {
                EditorUtility.DisplayDialog("Error", "You're terrain slices need to be aligned in a grid to be merged.", "Cancel");
                return;
            }

            MergeTerrainTiles(sorted, Selection, terrainsPerAxis);

            EditorUtility.ClearProgressBar();
        }

        private static int Clamp(int xTarget, int maximum)
        {
            return xTarget < 0 ? 0 : xTarget >= maximum ? maximum - 1 : xTarget;
        }

        private void CopyChildren(Terrain[,] sorted, Terrain targetTerrain, int terrainsPerAxis)
        {
            for (var ty = 0; ty < terrainsPerAxis; ty++)
            {
                for (var tx = 0; tx < terrainsPerAxis; tx++)
                {
                    foreach (Transform child in sorted[ty, tx].transform)
                    {
                        UnityEditor.Selection.activeGameObject = child.gameObject;
                        Unsupported.CopyGameObjectsToPasteboard();
                        Unsupported.PasteGameObjectsFromPasteboard();
                        GameObject copy = UnityEditor.Selection.activeGameObject;
                        copy.transform.SetParent(targetTerrain.transform);
                        copy.name = child.name;
                    }
                }
            }
        }

        private void CopyParentProperties(Terrain[] sourceTerrains, Terrain targetTerrain)
        {
            targetTerrain.basemapDistance = sourceTerrains.Average(x => x.basemapDistance);
            targetTerrain.shadowCastingMode = sourceTerrains.First().shadowCastingMode;
            targetTerrain.detailObjectDensity = sourceTerrains.Average(x => x.detailObjectDensity);
            targetTerrain.detailObjectDistance = sourceTerrains.Average(x => x.detailObjectDistance);
            targetTerrain.heightmapMaximumLOD = (int) sourceTerrains.Average(x => x.heightmapMaximumLOD);
            targetTerrain.heightmapPixelError = sourceTerrains.Average(x => x.heightmapPixelError);
            targetTerrain.treeBillboardDistance = sourceTerrains.Average(x => x.treeBillboardDistance);
            targetTerrain.treeCrossFadeLength = sourceTerrains.Average(x => x.treeCrossFadeLength);
            targetTerrain.treeDistance = sourceTerrains.Average(x => x.treeDistance);
            targetTerrain.treeMaximumFullLODCount = (int) sourceTerrains.Average(x => x.treeMaximumFullLODCount);
            targetTerrain.bakeLightProbesForTrees = sourceTerrains.Any(x => x.bakeLightProbesForTrees);
            targetTerrain.drawInstanced = sourceTerrains.Any(x => x.drawInstanced);
            targetTerrain.reflectionProbeUsage = sourceTerrains.First().reflectionProbeUsage;
            targetTerrain.realtimeLightmapScaleOffset = sourceTerrains.First().realtimeLightmapScaleOffset;
            targetTerrain.lightmapScaleOffset = sourceTerrains.First().lightmapScaleOffset;

            SetLightmapScale(targetTerrain, (int) Math.Sqrt(sourceTerrains.Length), sourceTerrains.First());

            targetTerrain.terrainData.wavingGrassAmount = sourceTerrains.Average(x => x.terrainData.wavingGrassAmount);
            targetTerrain.terrainData.wavingGrassSpeed = sourceTerrains.Average(x => x.terrainData.wavingGrassSpeed);
            targetTerrain.terrainData.wavingGrassStrength = sourceTerrains.Average(x => x.terrainData.wavingGrassStrength);
            targetTerrain.terrainData.wavingGrassTint = sourceTerrains.First().terrainData.wavingGrassTint;
        }

        private static void CreateHeightmap(Terrain[] sourceTerrains, Terrain[,] sorted, Terrain targetTerrain, int piecesPerAxis)
        {
            TerrainData targetTerrainData = targetTerrain.terrainData;
            var maxHeightmapResolution = sourceTerrains.Max(x => x.terrainData.heightmapResolution);
            maxHeightmapResolution = Math.Min(maxHeightmapResolution, MAXIMUM_HEIGHTMAP_RESOLUTION);

            var targetHeightmapResolution = (maxHeightmapResolution - 1) * piecesPerAxis + 1;
            targetTerrainData.heightmapResolution = targetHeightmapResolution;

            //Keep y same
            targetTerrainData.size = new Vector3(sourceTerrains.Max(x => x.terrainData.size.x) * piecesPerAxis, sourceTerrains.Max(x => x.terrainData.size.y),
                sourceTerrains.Max(x => x.terrainData.size.z) * piecesPerAxis);

            var mergedHeights = new float[targetHeightmapResolution, targetHeightmapResolution];

            for (var sliceIndex = 0; sliceIndex < piecesPerAxis * piecesPerAxis; sliceIndex++)
            {
                var tx = sliceIndex % piecesPerAxis;
                var ty = sliceIndex / piecesPerAxis;
                TerrainData terrainData = sorted[ty, tx].terrainData;
                var sourceResolution = terrainData.heightmapResolution;
                var sourceResolutionMinus1 = sourceResolution - 1;
                var parentHeights = terrainData.GetHeights(0, 0, sourceResolution, sourceResolution);

                var targetRegionWidth = (targetHeightmapResolution - 1) / piecesPerAxis;

                for (var x = 0; x <= targetRegionWidth; x++)
                {
                    for (var y = 0; y <= targetRegionWidth; y++)
                    {
                        var xPos = x / (float) targetRegionWidth * sourceResolution;
                        var yPos = y / (float) targetRegionWidth * sourceResolution;

                        var ph = BilinearInterpolator.Interpolate(parentHeights, xPos, yPos, sourceResolutionMinus1);

                        mergedHeights[x + targetRegionWidth * tx, y + targetRegionWidth * ty] = ph;
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            // Set heightmap to child
            targetTerrain.terrainData.SetHeights(0, 0, mergedHeights);
        }

        private static Terrain[,] DetermineTerrainPositions(Terrain[] sourceTerrains, int piecesPerAxis)
        {
            var sorted = new Terrain[piecesPerAxis, piecesPerAxis];

            var bounds = new Bounds(sourceTerrains.First().GetPosition(), Vector3.zero);
            for (var index = 1; index < sourceTerrains.Length; index++)
            {
                Terrain sourceTerrain = sourceTerrains[index];
                Vector3 pos = sourceTerrain.GetPosition();
                bounds.Encapsulate(pos);
            }

            if (bounds.extents.y != 0)
                throw new Exception("bounds extent was " + bounds.extents);

            foreach (Terrain sourceTerrain in sourceTerrains)
            {
                Vector3 boundsMin = sourceTerrain.GetPosition() - bounds.min;
                var index = new Vector2(boundsMin.x / bounds.size.x, boundsMin.z / bounds.size.z);

                var x = (int) Math.Round(index.x * (piecesPerAxis - 1));
                var y = (int) Math.Round(index.y * (piecesPerAxis - 1));

                sorted[x, y] = sourceTerrain;
            }

            return sorted;
        }

        private void FillMergeDetails(TerrainData td, Terrain[,] sorted, Terrain[] sourceTerrains, Terrain targetTerrain, int terrainsPerAxis)
        {
            var maxDetailResolution = sourceTerrains.Max(x => x.terrainData.detailResolution);
            var maxDetailResolutionPerPath = sourceTerrains.Max(x => x.terrainData.detailResolutionPerPatch);

            var newDetailResolution = maxDetailResolution * terrainsPerAxis;

            td.SetDetailResolution(newDetailResolution, maxDetailResolutionPerPath);

            for (var targetLayer = 0; targetLayer < targetTerrain.terrainData.detailPrototypes.Length; targetLayer++)
            {
                var targetDetails = new int[newDetailResolution, newDetailResolution];
                DetailPrototype targetDetailObject = targetTerrain.terrainData.detailPrototypes[targetLayer];

                for (var ty = 0; ty < terrainsPerAxis; ty++)
                {
                    for (var tx = 0; tx < terrainsPerAxis; tx++)
                    {
                        TerrainData currentTerrainData = sorted[ty, tx].terrainData;
                        var currentDetailResolution = currentTerrainData.detailResolution;

                        var sourceLayerIndices = IndexOf(currentTerrainData.detailPrototypes, targetDetailObject,
                            (a, b) => Equals(a?.prototypeTexture, b?.prototypeTexture) && Equals(a?.prototype, b?.prototype));

                        foreach (var sourceLayerIndex in sourceLayerIndices)
                        {
                            var sourceDetails = currentTerrainData.GetDetailLayer(0, 0, currentDetailResolution, currentDetailResolution, sourceLayerIndex);

                            // Shift calc

                            var xShift = tx * maxDetailResolution;
                            var yShift = ty * maxDetailResolution;
                            var scale = maxDetailResolution / currentDetailResolution;

                            // iterate				
                            for (var x = 0; x < currentDetailResolution; x++)
                            {
                                for (var y = 0; y < currentDetailResolution; y++)
                                    RunDetailsKernel(sourceDetails, x, y, scale, xShift, yShift, newDetailResolution, targetDetails, terrainsPerAxis);
                            }
                        }

                        EditorUtility.ClearProgressBar();
                    }
                }

                // Set heightmap to child
                targetTerrain.terrainData.SetDetailLayer(0, 0, targetLayer, targetDetails);
            }
        }

        private void FillSplatMaps(TerrainData td, Terrain[,] sorted, Terrain targetTerrain, int piecesPerAxis)
        {
            var terrainDatas = sorted.Cast<Terrain>().Select(x => x.terrainData).ToArray();
            var sourceAlphamapResolution = terrainDatas.Max(x => x.alphamapResolution);
            var sourceBasemapResolution = terrainDatas.Max(x => x.baseMapResolution);

            var newAlphamapResolution = Math.Min(MAXIMUM_CONTROL_TEXTURE_RESOLUTION, sourceAlphamapResolution * piecesPerAxis);
            var newBasemapResolution = Math.Min(MAXIMUM_BASEMAP_TEXTURE_RESOLUTION, sourceBasemapResolution * piecesPerAxis);

            td.alphamapResolution = newAlphamapResolution;
            td.baseMapResolution = newBasemapResolution;

            var result = new float[newAlphamapResolution, newAlphamapResolution, targetTerrain.terrainData.terrainLayers.Length];

            var progress = 0;
            var targetRegionWidth = newAlphamapResolution / piecesPerAxis;
            var max = terrainDatas.Max(x => x.alphamapLayers) * targetRegionWidth * targetRegionWidth * piecesPerAxis * piecesPerAxis;

            for (var sliceIndex = 0; sliceIndex < piecesPerAxis * piecesPerAxis; sliceIndex++)
            {
                var tx = sliceIndex % piecesPerAxis;
                var ty = sliceIndex / piecesPerAxis;
                TerrainData terrainData = sorted[ty, tx].terrainData;
                var sourceResolution = terrainData.alphamapResolution;
                var sourceResolutionMinus1 = sourceResolution - 1;
                var parentHeights = terrainData.GetAlphamaps(0, 0, sourceResolution, sourceResolution);

                var alphamapLayers = terrainData.alphamapLayers;

                for (var sourceIndex = 0; sourceIndex < alphamapLayers; sourceIndex++)
                {
                    var targetIndex = IndexOf(targetTerrain.terrainData.terrainLayers, terrainData.terrainLayers[sourceIndex]).First();

                    for (var x = 0; x < targetRegionWidth; x++)
                    {
                        for (var y = 0; y < targetRegionWidth; y++)
                        {
                            var xPos = x / (float) targetRegionWidth * sourceResolution;
                            var yPos = y / (float) targetRegionWidth * sourceResolution;

                            var ph = BilinearInterpolator.Interpolate(parentHeights, sourceIndex, xPos, yPos, sourceResolutionMinus1);

                            result[x + targetRegionWidth * tx, y + targetRegionWidth * ty, targetIndex] = ph;

                            if (progress % 1000 == 0)
                                EditorUtility.DisplayProgressBar("Writing alpha map value", $"Working {progress}/{max}...", progress / (float) max);
                            progress++;
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            // Set heightmap to child
            targetTerrain.terrainData.SetAlphamaps(0, 0, result);
        }

        private List<int> IndexOf<T>(T[] stack, T needle, Func<T, T, bool> equals = null)
        {
            if (equals == null)
                equals = (a, b) => Equals(a, b);

            var matches = new List<int>();

            for (var i = 0; i < stack.Length; i++)
            {
                if (equals(needle, stack[i]))
                    matches.Add(i);
            }

            return matches;
        }

        private static bool IsPowerOf2(int value)
        {
            return (value & (value - 1)) == 0;
        }

        private bool IsValid()
        {
            var selectionCount = Selection.Length;

            if (selectionCount < 4)
                return false;

            var piecesPerAxis = Math.Sqrt(selectionCount);

            if (piecesPerAxis - Math.Floor(piecesPerAxis) > 0.001f)
                return false;

            if (!IsPowerOf2((int) piecesPerAxis))
                return false;

            return true;
        }

        private static T[] MergeArray<T>(Terrain[] sourceTerrains, Func<Terrain, T[]> retrieve)
        {
            return sourceTerrains.SelectMany(retrieve).Distinct().ToArray();
        }

        private TreePrototype[] MergeSimiliarPrototypes(TreePrototype[] prototypes)
        {
            var result = new List<TreePrototype>();

            foreach (TreePrototype prototype in prototypes)
            {
                TreePrototype alreadyCreated = result.FirstOrDefault(x => x.prefab != null && x.prefab == prototype.prefab);

                if (alreadyCreated == null)
                {
                    result.Add(prototype);
                    alreadyCreated = prototype;
                }

                alreadyCreated.bendFactor = Math.Max(alreadyCreated.bendFactor, prototype.bendFactor);
            }

            return result.ToArray();
        }

        private DetailPrototype[] MergeSimiliarPrototypes(DetailPrototype[] prototypes)
        {
            var result = new List<DetailPrototype>();

            foreach (DetailPrototype prototype in prototypes)
            {
                DetailPrototype alreadyCreated = result.FirstOrDefault(x =>
                    x.prototypeTexture != null && x.prototypeTexture == prototype.prototypeTexture ||
                    x.prototype != null && x.prototype == prototype.prototype);

                if (alreadyCreated == null)
                {
                    result.Add(prototype);
                    alreadyCreated = prototype;
                }

                alreadyCreated.minHeight = Math.Min(alreadyCreated.minHeight, prototype.minHeight);
                alreadyCreated.minWidth = Math.Min(alreadyCreated.minWidth, prototype.minWidth);
                alreadyCreated.maxHeight = Math.Max(alreadyCreated.maxHeight, prototype.maxHeight);
                alreadyCreated.maxWidth = Math.Max(alreadyCreated.maxWidth, prototype.maxWidth);
            }

            return result.ToArray();
        }

        private void MergeTerrainTiles(Terrain[,] sorted, Terrain[] sourceTerrains, int terrainsPerAxis)
        {
            var terrainData = new TerrainData();
            GameObject tgo = Terrain.CreateTerrainGameObject(terrainData);
            var targetTerrain = tgo.GetComponent<Terrain>();
            targetTerrain.terrainData = terrainData;
            tgo.name = sourceTerrains.First().name + " ";
            AssetDatabase.CreateAsset(terrainData, "Assets/" + targetTerrain.name + ".asset");

            // Assign splatmaps
            targetTerrain.terrainData.terrainLayers = MergeArray(sourceTerrains, x => x.terrainData.terrainLayers);

            // Assign detail prototypes
            var prototypes = MergeArray(sourceTerrains, x => x.terrainData.detailPrototypes);
            targetTerrain.terrainData.detailPrototypes = MergeSimiliarPrototypes(prototypes);

            // Assign tree information
            var treePrototypes = MergeArray(sourceTerrains, x => x.terrainData.treePrototypes);
            targetTerrain.terrainData.treePrototypes = MergeSimiliarPrototypes(treePrototypes);

            CopyParentProperties(sourceTerrains, targetTerrain);

            //Start processing it			
            tgo.transform.position = sorted[0, 0].GetPosition();

            //Copy heightmap											
            CreateHeightmap(sourceTerrains, sorted, targetTerrain, terrainsPerAxis);

            // Merge splat map
            FillSplatMaps(terrainData, sorted, targetTerrain, terrainsPerAxis);

            // Merge detail map
            FillMergeDetails(terrainData, sorted, sourceTerrains, targetTerrain, terrainsPerAxis);

            AssetDatabase.SaveAssets();

            // Merge tree data
            MergeTrees(sorted, targetTerrain, terrainsPerAxis);

            CopyChildren(sorted, targetTerrain, terrainsPerAxis);

            AssetDatabase.SaveAssets();

            foreach (Terrain sourceTerrain in sourceTerrains)
                sourceTerrain.gameObject.SetActive(false);
        }

        private void MergeTrees(Terrain[,] sorted, Terrain targetTerrain, int terrainsPerAxis)
        {
            for (var ty = 0; ty < terrainsPerAxis; ty++)
            {
                for (var tx = 0; tx < terrainsPerAxis; tx++)
                {
                    var terrainDataTreeInstances = sorted[ty, tx].terrainData.treeInstances;
                    for (var t = 0; t < terrainDataTreeInstances.Length; t++)
                    {
                        // Get tree instance					
                        TreeInstance ti = terrainDataTreeInstances[t];

                        // Recalculate new tree position	
                        ti.position = new Vector3(ti.position.x / terrainsPerAxis + ty / (float) terrainsPerAxis, ti.position.y,
                            ti.position.z / terrainsPerAxis + tx / (float) terrainsPerAxis);

                        // Add tree instance						
                        targetTerrain.AddTreeInstance(ti);
                    }
                }
            }
        }

        private static void RunDetailsKernel(int[,] sourceDetails, int x, int y, int scale, int xShift, int yShift, int newDetailResolution,
            int[,] targetDetails, int terrainsPerAxis)
        {
            var ph = sourceDetails[x, y];

            if (ph == 0)
                return;

            var scaleBoundUpper = Math.Max(1, scale / terrainsPerAxis);
            var scaleBoundLower = -scale / terrainsPerAxis;

            for (var xScale = scaleBoundLower; xScale < scaleBoundUpper; xScale++)
            {
                for (var yScale = scaleBoundLower; yScale < scaleBoundUpper; yScale++)
                {
                    var xTarget = x * scale + xShift + xScale;
                    var yTarget = y * scale + yShift + yScale;

                    xTarget = Clamp(xTarget, newDetailResolution);
                    yTarget = Clamp(yTarget, newDetailResolution);

                    targetDetails[xTarget, yTarget] += Math.Max(1, ph / scale);
                }
            }
        }

        private void SetLightmapScale(Terrain targetTerrain, float terrainPiecesPerAxis, Terrain sourceTerrain)
        {
            const string scaleInLightmap = "m_ScaleInLightmap";

            var sos = new SerializedObject(sourceTerrain);
            var sourceValue = sos.FindProperty(scaleInLightmap).floatValue;
            sos.ApplyModifiedProperties();

            var so = new SerializedObject(targetTerrain);
            so.FindProperty(scaleInLightmap).floatValue = sourceValue * terrainPiecesPerAxis;
            so.ApplyModifiedProperties();
        }
    }
}