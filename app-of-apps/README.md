kubectl create ns dom1
helm upgrade --install app1 . -f values.yaml -n dom1
helm uninstall app1 -n dom1

kubectl apply -f app1/argoapp/app1.yaml
kubectl delete -f app1/argoapp/app1.yaml