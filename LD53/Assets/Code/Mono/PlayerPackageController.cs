﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Code
{
    public class PlayerPackageController : MonoBehaviour
    {
        public Action OnPackageContainerChanged;
        [SerializeField] private List<PackageContainer> _packageContainers;
        [SerializeField] private List<Package> _packages;
        private PackageContainer _currentPackageContainer;
        public float collectPackageRange = 10;
        public float enemyDetectRange = 10;

        private void Awake()
        {
            _currentPackageContainer = GetPackageContainer(_packages.Count);
            _currentPackageContainer.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckPackages();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CheckEnemy();
            }
        }

        private void CheckEnemy()
        {
            var col = Physics2D.OverlapCircle(transform.position, enemyDetectRange,
                LayerConstants.EnemyLayerMask);

            if (col)
            {
                AttackWithPackage(col.transform);
            }
        }

        private void AttackWithPackage(Transform enemyTransform)
        {
            if (_packages.Count <= 1)
            {
                Debug.LogWarning("Cant remove package. We only have one");
                return;
            }

            int packageIndex = _packages.Count - 1;
            Package packageToRemove = _packages[packageIndex];
            packageToRemove.transform.DOKill();
            packageToRemove.transform.SetParent(null);
            packageToRemove.transform.DOMove(enemyTransform.position, 0.5f).OnComplete(ReArrangePackages);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, collectPackageRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyDetectRange);
        }

        private void AddPackage(Package package)
        {
            if (_packages.Contains(package))
            {
                Debug.LogWarning("You are trying to add package that already added!");
                return;
            }

            _packages.Add(package);
            package.SetPackageState(PackageState.Picked);

            ReArrangePackages();
        }

        private void ReArrangePackages()
        {
            PackageContainer packageContainer = GetPackageContainer(_packages.Count);
            if (packageContainer != _currentPackageContainer)
            {
                _currentPackageContainer.gameObject.SetActive(false);
                packageContainer.gameObject.SetActive(true);
                _currentPackageContainer = packageContainer;
                OnPackageContainerChanged?.Invoke();
            }

            packageContainer.ArrangePackages(_packages);
        }

        public void RemovePackage()
        {
            if (_packages.Count <= 1)
            {
                Debug.LogWarning("Cant remove package. We only have one");
                return;
            }

            int packageIndex = _packages.Count - 1;
            Package packageToRemove = _packages[packageIndex];
            _packages.RemoveAt(packageIndex);
            packageToRemove.transform.DOKill();
            packageToRemove.transform.SetParent(null);
            packageToRemove.SetPackageState(PackageState.Free);
            ReArrangePackages();
            packageToRemove.SetForce(Vector2.left * 10);
        }

        private PackageContainer GetPackageContainer(int packageCount)
        {
            PackageContainer defaultContainer = _packageContainers[0];
            PackageContainer tempContainer = null;
            switch (packageCount)
            {
                case 1:
                    tempContainer = _packageContainers.Find((x) =>
                        x.GetPackageContainerType() == PackageContainerType.OnePackage);
                    if (tempContainer != null)
                    {
                        return tempContainer;
                    }

                    break;
                case 2:
                    tempContainer = _packageContainers.Find((x) =>
                        x.GetPackageContainerType() == PackageContainerType.TwoPackage);
                    if (tempContainer != null)
                    {
                        return tempContainer;
                    }

                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    tempContainer = _packageContainers.Find((x) =>
                        x.GetPackageContainerType() == PackageContainerType.ThreeAndNinePackage);
                    if (tempContainer != null)
                    {
                        return tempContainer;
                    }

                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    tempContainer = _packageContainers.Find((x) =>
                        x.GetPackageContainerType() == PackageContainerType.TenAndFourteenPackage);
                    if (tempContainer != null)
                    {
                        return tempContainer;
                    }

                    break;
                default:
                    tempContainer = _packageContainers.Find((x) =>
                        x.GetPackageContainerType() == PackageContainerType.ManyPackage);
                    if (tempContainer != null)
                    {
                        return tempContainer;
                    }

                    break;
            }

            return defaultContainer;
        }

        private void CheckPackages()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, collectPackageRange,
                LayerConstants.PackageLayerMask);
            if (colliders == null || colliders.Length == 0)
            {
                return;
            }

            foreach (var col in colliders)
            {
                Package package = col.GetComponent<Package>();
                if (package == null)
                {
                    Debug.LogWarning($"Package not found! {GetComponent<Collider>().gameObject.name}");
                    return;
                }

                if (package.GetPackageState() == PackageState.Free)
                {
                    AddPackage(package);
                    return; //One is enough for one input.
                }
            }
        }
    }
}